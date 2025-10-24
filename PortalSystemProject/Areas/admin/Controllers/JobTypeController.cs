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
    public class JobTypeController : Controller
    {
        private readonly PortalContext _context;

        public JobTypeController(PortalContext context)
        {
            _context = context;
        }

        // GET: Admin/JobType
        public async Task<IActionResult> Index()
        {
            var jobTypes = await _context.JobTypes
                .Select(jt => new JobTypeItemViewModel
                {
                    Id = jt.Id,
                    Name = jt.Name,
                    JobCount = jt.JobPosts.Count,
                    CreatedAt = jt.CreatedDate ?? DateTime.Now
                })
                .ToListAsync();

            var viewModel = new JobTypeListViewModel
            {
                JobTypes = jobTypes,
                TotalCount = jobTypes.Count
            };

            return View(viewModel);
        }

        // GET: Admin/JobType/Create
        public IActionResult Create()
        {
            return View(new CreateJobTypeViewModel());
        }

        // POST: Admin/JobType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var exists = await _context.JobTypes
                    .AnyAsync(jt => jt.Name.ToLower() == model.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "Job type already exists");
                    return View(model);
                }

                var jobType = new JobType
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    CreatedDate = DateTime.Now,
                    CurrentState = 1
                };

                _context.JobTypes.Add(jobType);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Job type created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // GET: Admin/JobType/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var jobType = await _context.JobTypes.FindAsync(id);
            if (jobType == null)
            {
                return NotFound();
            }

            var jobCount = await _context.JobPosts.CountAsync(j => j.JobTypeId == id);

            var viewModel = new EditJobTypeViewModel
            {
                Id = jobType.Id,
                Name = jobType.Name,
                JobCount = jobCount
            };

            return View(viewModel);
        }

        // POST: Admin/JobType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditJobTypeViewModel model)
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
                var jobType = await _context.JobTypes.FindAsync(id);
                if (jobType == null)
                {
                    return NotFound();
                }

                var exists = await _context.JobTypes
                    .AnyAsync(jt => jt.Name.ToLower() == model.Name.ToLower() && jt.Id != id);

                if (exists)
                {
                    ModelState.AddModelError("Name", "Job type name already exists");
                    return View(model);
                }

                jobType.Name = model.Name;
                jobType.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Job type updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // POST: Admin/JobType/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var jobType = await _context.JobTypes.FindAsync(id);
                if (jobType == null)
                {
                    return Json(new { success = false, message = "Job type not found" });
                }

                var hasJobs = await _context.JobPosts.AnyAsync(j => j.JobTypeId == id);
                if (hasJobs)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete job type with existing jobs"
                    });
                }

                _context.JobTypes.Remove(jobType);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Job type deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}