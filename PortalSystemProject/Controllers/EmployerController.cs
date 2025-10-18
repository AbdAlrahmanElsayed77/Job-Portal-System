using BL.Contracts;
using BL.Dtos;
using BL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PortalSystemProject.Controllers
{
    public class EmployerController : Controller
    {
        public EmployerController
            (
            IJobPostRepository jobPostService,
            IEmployerProfileRepository employerProfileService,
            ICompanyRepository companyService,
            IJobCategoryRepository categoryService,
            IJobTypeRepository jobTypeService
            )
        {
            JobPostService = jobPostService;
            EmployerProfileService = employerProfileService;
            CompanyService = companyService;
            CategoryService = categoryService;
            JobTypeService = jobTypeService;
            //EmpId = GetCurrentEmployerId();
            //EmpId = GetCurrentUserId();
        }

        public IJobPostRepository JobPostService { get; }
        public IEmployerProfileRepository EmployerProfileService { get; }
        public ICompanyRepository CompanyService { get; }
        public IJobCategoryRepository CategoryService { get; }
        public IJobTypeRepository JobTypeService { get; }

        Guid EmpId;

        public IActionResult Index()
        {
           return Content(GetCurrentUserId().ToString());
        }


        //view the jops for employer
        public IActionResult JobPosts() 
        {
            //get the jops for the current employer
            var jopPosts = JobPostService.getJopsForEmployer(GetCurrentUserId());
            return View(jopPosts);
        }

        public IActionResult create()
        {
            // Load dropdown data
            ViewBag.JobCategories = CategoryService.GetAll();
            ViewBag.JobTypes = JobTypeService.GetAll();

            return View();
        }
        [HttpPost]
        public IActionResult create(JobPostDto jop)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.JobCategories = CategoryService.GetAll();
                ViewBag.JobTypes = JobTypeService.GetAll();
                return View(jop);
            }
            var currentEmployer = EmployerProfileService.GetById(EmpId);

            //jop.CompanyId= currentEmployer.CompanyId;
            //jop.CreatedByUserId = currentEmployer.Id;
            jop.CreatedByUserId = GetCurrentUserId();
            jop.PublishedAt = DateTime.Now;
            JobPostService.Add(jop);
            TempData["success"] = "Job post added successfully!";
            return RedirectToAction("JobPosts");
        }

        //edit post 
        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var jobPost = JobPostService.GetById(id);
            if (jobPost == null)
                return NotFound();
            ViewBag.JobCategories = CategoryService.GetAll();
            ViewBag.JobTypes = JobTypeService.GetAll();
            return View(jobPost);
        }
        [HttpPost]
        public IActionResult Edit(JobPostDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.JobCategories = CategoryService.GetAll();
                ViewBag.JobTypes = JobTypeService.GetAll();
                return View(model);
            }
            model.CreatedByUserId = GetCurrentUserId();
            JobPostService.Update(model);
            return RedirectToAction("JobPosts");
        }

        // Delete job post
        public IActionResult Delete(Guid id)
        {
            var job = JobPostService.GetById(id);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(Guid id)
        {
            JobPostService.ChangeStatus(id); // optional: mark inactive
            return RedirectToAction("JobPosts");
        }







        //get the id of the logged in user
        private Guid GetCurrentUserId()
        {
            var id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return  id ;
        }

        private Guid GetCurrentEmployerId()
        {
            var employer = EmployerProfileService
                .GetAll()
                .FirstOrDefault(e => e.UserId == GetCurrentUserId());
            return employer?.Id ?? Guid.Empty;
        }
    }
}
