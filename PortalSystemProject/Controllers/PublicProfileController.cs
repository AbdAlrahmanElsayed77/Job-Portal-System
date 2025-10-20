using BL.Contracts;
using DAL.Contracts;
using Domains;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortalSystemProject.Controllers
{
    public class PublicProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITableRepository<JobSeekerProfile> _jobSeekerRepo;
        private readonly ITableRepository<EmployerProfile> _employerRepo;

        public PublicProfileController(
            UserManager<ApplicationUser> userManager,
            ITableRepository<JobSeekerProfile> jobSeekerRepo,
            ITableRepository<EmployerProfile> employerRepo)
        {
            _userManager = userManager;
            _jobSeekerRepo = jobSeekerRepo;
            _employerRepo = employerRepo;
        }

        // GET: /PublicProfile/{userId}
        [HttpGet("PublicProfile/{userId}")]
        public async Task<IActionResult> Index(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index", "Home");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                ViewBag.UserEmail = user.Email;
                ViewBag.MemberSince = user.EmailConfirmed ? "Verified Member" : "Member";

                if (role == "JobSeeker")
                {
                    var profile = _jobSeekerRepo.GetAll(p => p.CVFiles)
                        .FirstOrDefault(p => p.UserId == userId);

                    if (profile == null)
                    {
                        ViewBag.NoProfile = true;
                        ViewBag.UserRole = "JobSeeker";
                        return View("NoProfile");
                    }

                    ViewBag.UserRole = "JobSeeker";
                    return View("JobSeekerPublic", profile);
                }
                else if (role == "Employer")
                {
                    var profile = _employerRepo.GetAll(p => p.Company)
                        .FirstOrDefault(p => p.UserId == userId);

                    if (profile == null)
                    {
                        ViewBag.NoProfile = true;
                        ViewBag.UserRole = "Employer";
                        return View("NoProfile");
                    }

                    ViewBag.UserRole = "Employer";
                    return View("EmployerPublic", profile);
                }

                TempData["Error"] = "Invalid user type";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading profile: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}