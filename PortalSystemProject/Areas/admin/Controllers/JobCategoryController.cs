using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.DbContext;
using Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalSystemProject.Models.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortalSystemProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class JobCategoryController : Controller
    {
        private readonly PortalContext _context;
        private readonly IJobCategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public JobCategoryController(
            PortalContext context,
            IJobCategoryRepository categoryRepo,
            IMapper mapper)
        {
            _context = context;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        // GET: Admin/JobCategory
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetCategoriesWithJobCountAsync();

            var viewModel = new JobCategoryListViewModel
            {
                Categories = categories.Select(c => new JobCategoryItemViewModel
                {
                    Id = c.Category.Id,
                    Name = c.Category.Name,
                    JobCount = c.JobCount,
                    CreatedAt = DateTime.Now // TODO: Add from entity
                }).ToList(),
                TotalCount = categories.Count
            };

            return View(viewModel);
        }

        // GET: Admin/JobCategory/Create
        public IActionResult Create()
        {
            return View(new CreateJobCategoryViewModel());
        }

        // POST: Admin/JobCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if category already exists
                var exists = await _context.JobCategories
                    .AnyAsync(c => c.Name.ToLower() == model.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "Category already exists");
                    return View(model);
                }

                var category = new JobCategory
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    CreatedDate = DateTime.Now,
                    CurrentState = 1
                };

                _context.JobCategories.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // GET: Admin/JobCategory/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _context.JobCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var jobCount = await _context.JobPosts.CountAsync(j => j.JobCategoryId == id);

            var viewModel = new EditJobCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                JobCount = jobCount
            };

            return View(viewModel);
        }

        // POST: Admin/JobCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditJobCategoryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var category = await _context.JobCategories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                // Check if name already exists (excluding current)
                var exists = await _context.JobCategories
                    .AnyAsync(c => c.Name.ToLower() == model.Name.ToLower() && c.Id != id);

                if (exists)
                {
                    ModelState.AddModelError("Name", "Category name already exists");
                    return View(model);
                }

                category.Name = model.Name;
                category.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // POST: Admin/JobCategory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var category = await _context.JobCategories.FindAsync(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Category not found" });
                }

                // Check if category has jobs
                var hasJobs = await _context.JobPosts.AnyAsync(j => j.JobCategoryId == id);
                if (hasJobs)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete category with existing jobs"
                    });
                }

                _context.JobCategories.Remove(category);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}