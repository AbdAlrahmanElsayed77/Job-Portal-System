using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace PortalSystemProject.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ITableRepository<Company> _companyRepo;
        private readonly ITableRepository<JobPost> _jobPostRepo;
        private readonly IFileService _fileService;

        public CompanyController(
            ITableRepository<Company> companyRepo,
            ITableRepository<JobPost> jobPostRepo,
            IFileService fileService)
        {
            _companyRepo = companyRepo;
            _jobPostRepo = jobPostRepo;
            _fileService = fileService;
        }

        // GET: /Company - Browse all companies (Public)
        [HttpGet]
        public IActionResult Index(string search = "")
        {
            try
            {
                var companies = _companyRepo.GetAll()
                    .Where(c => c.Status == 1); // Only approved companies

                if (!string.IsNullOrEmpty(search))
                {
                    companies = companies.Where(c =>
                        c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (c.Description != null && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                ViewBag.SearchTerm = search;
                return View(companies.ToList());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading companies: " + ex.Message;
                return View(new List<Company>());
            }
        }

        // GET: /Company/Details/{id} - View company details (Public)
        [HttpGet]
        public IActionResult Details(Guid id)
        {
            try
            {
                var company = _companyRepo.GetById(id);
                if (company == null)
                {
                    TempData["Error"] = "Company not found";
                    return RedirectToAction("Index");
                }

                // Get job posts for this company
                var jobPosts = _jobPostRepo.GetAll(j => j.Company)
                    .Where(j => j.CompanyId == id && j.CurrentState == 1)
                    .OrderByDescending(j => j.CreatedDate)
                    .ToList();

                ViewBag.JobPosts = jobPosts;
                ViewBag.ActiveJobsCount = jobPosts.Count;

                return View(company);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading company: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: /Company/MyCompany - Employer's company management
        [HttpGet]
        [Authorize(Roles = "Employer")]
        public IActionResult MyCompany()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // Find company created by this employer
                var company = _companyRepo.GetAll()
                    .FirstOrDefault(c => c.CreatedBy == userId);

                if (company == null)
                {
                    TempData["Info"] = "You haven't created a company yet. Create one now!";
                    return RedirectToAction("Create");
                }

                // Get statistics
                var jobPosts = _jobPostRepo.GetAll()
                    .Where(j => j.CompanyId == company.Id)
                    .ToList();

                ViewBag.TotalJobs = jobPosts.Count;
                ViewBag.ActiveJobs = jobPosts.Count(j => j.CurrentState == 1);
                ViewBag.RecentJobs = jobPosts.OrderByDescending(j => j.CreatedDate).Take(5).ToList();

                return View(company);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading company: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Company/Create - Create new company (Employer only)
        [HttpGet]
        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Check if employer already has a company
            var existingCompany = _companyRepo.GetAll()
                .FirstOrDefault(c => c.CreatedBy == userId);

            if (existingCompany != null)
            {
                TempData["Warning"] = "You already have a company. You can edit it instead.";
                return RedirectToAction("Edit", new { id = existingCompany.Id });
            }

            return View(new Company());
        }

        // POST: /Company/Create
        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Company model, IFormFile? logoFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // Check for duplicate company name
                var existingCompany = _companyRepo.GetAll()
                    .FirstOrDefault(c => c.Name.ToLower() == model.Name.ToLower());

                if (existingCompany != null)
                {
                    ModelState.AddModelError("Name", "A company with this name already exists");
                    return View(model);
                }

                model.Id = Guid.NewGuid();
                model.CreatedBy = userId;
                model.CreatedDate = DateTime.UtcNow;
                model.CurrentState = 1;
                model.Status = 1; // Approved by default

                // Upload logo if provided
                if (logoFile != null)
                {
                    model.LogoUrl = _fileService.UploadFileAsync("company_logos", logoFile).Result;
                }

                _companyRepo.Add(model);
                TempData["Success"] = "Company created successfully!";
                return RedirectToAction("MyCompany");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating company: " + ex.Message;
                return View(model);
            }
        }

        // GET: /Company/Edit/{id} - Edit company (Employer only)
        [HttpGet]
        [Authorize(Roles = "Employer")]
        public IActionResult Edit(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var company = _companyRepo.GetById(id);

                if (company == null)
                {
                    TempData["Error"] = "Company not found";
                    return RedirectToAction("MyCompany");
                }

                // Verify ownership
                if (company.CreatedBy != userId)
                {
                    TempData["Error"] = "You don't have permission to edit this company";
                    return RedirectToAction("MyCompany");
                }

                return View(company);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading company: " + ex.Message;
                return RedirectToAction("MyCompany");
            }
        }

        // POST: /Company/Edit/{id}
        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Company model, IFormFile? logoFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var existingCompany = _companyRepo.GetById(model.Id);

                if (existingCompany == null)
                {
                    TempData["Error"] = "Company not found";
                    return RedirectToAction("MyCompany");
                }

                // Verify ownership
                if (existingCompany.CreatedBy != userId)
                {
                    TempData["Error"] = "You don't have permission to edit this company";
                    return RedirectToAction("MyCompany");
                }

                // Preserve original data
                model.CreatedBy = existingCompany.CreatedBy;
                model.CreatedDate = existingCompany.CreatedDate;
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.UtcNow;

                // Handle logo upload
                if (logoFile != null)
                {
                    model.LogoUrl = _fileService.UploadFileAsync("company_logos", logoFile).Result;
                }
                else
                {
                    model.LogoUrl = existingCompany.LogoUrl;
                }

                _companyRepo.Update(model);
                TempData["Success"] = "Company updated successfully!";
                return RedirectToAction("MyCompany");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating company: " + ex.Message;
                return View(model);
            }
        }

        // POST: /Company/Delete/{id} - Delete company (Employer only)
        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var company = _companyRepo.GetById(id);

                if (company == null)
                {
                    TempData["Error"] = "Company not found";
                    return RedirectToAction("MyCompany");
                }

                // Verify ownership
                if (company.CreatedBy != userId)
                {
                    TempData["Error"] = "You don't have permission to delete this company";
                    return RedirectToAction("MyCompany");
                }

                // Check if company has active job posts
                var hasJobs = _jobPostRepo.GetAll()
                    .Any(j => j.CompanyId == id);

                if (hasJobs)
                {
                    TempData["Warning"] = "Cannot delete company with existing job posts. Please delete all jobs first.";
                    return RedirectToAction("MyCompany");
                }

                _companyRepo.Delete(id);
                TempData["Success"] = "Company deleted successfully";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting company: " + ex.Message;
                return RedirectToAction("MyCompany");
            }
        }
    }
}