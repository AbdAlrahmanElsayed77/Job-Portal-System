using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using BL.Dtos.Common;
using DAL.DbContext;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PortalSystemProject.Areas.admin.Controllers
{
    [Area("admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly PortalContext _context;
        private readonly IMapper _mapper;

        public DashboardController(IAdminDashboardService dashboardService, PortalContext context, IMapper mapper)
        {
            _dashboardService = dashboardService;
            _context = context;
            _mapper = mapper;
        }

        // -------------------- Dashboard --------------------
        [HttpGet]
        public async Task<IActionResult> Index(string period = "all")
        {
            var data = await _dashboardService.GetDashboardSummaryAsync(period);

            ViewBag.SelectedPeriod = period;
            ViewBag.UserCount = data.UserCount;
            ViewBag.CompanyCount = data.CompanyCount;
            ViewBag.JobCount = data.JobCount;
            ViewBag.CategoryCount = data.CategoryCount;
            ViewBag.CategoryLabels = data.CategoryLabels;
            ViewBag.CategoryJobCounts = data.CategoryJobCounts;
            ViewBag.CompanyLabels = data.CompanyLabels;
            ViewBag.CompanyJobCounts = data.CompanyJobCounts;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData(string period = "all")
        {
            var data = await _dashboardService.GetDashboardSummaryAsync(period);
            return Json(data);
        }

        // -------------------- Employers --------------------
        [HttpGet]
        public IActionResult Employers(int page = 1, int pageSize = 10)
        {
            var query = _context.EmployerProfiles
                .Include(e => e.User)
                .Include(e => e.Company)
                .OrderByDescending(e => e.CreatedDate)
                .AsQueryable();

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResultDto<EmployerProfile>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateEmployerStatus([FromBody] EmployerStatusUpdateDto request)
        //{
        //    var employer = await _context.EmployerProfiles.FindAsync(request.Id);
        //    if (employer == null)
        //        return NotFound();

        //    if (!Enum.TryParse<EmployerStatus>(request.Status, true, out var parsedStatus))
        //        return BadRequest("Invalid status value.");

        //    employer.Status = parsedStatus;
        //    _context.EmployerProfiles.Update(employer);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Status updated successfully ✅" });
        //}

        public class EmployerStatusUpdateDto
        {
            public Guid Id { get; set; }
            public string Status { get; set; } = string.Empty;
        }


        // -------------------- Jobs --------------------
        [HttpGet]
        public IActionResult Jobs(int page = 1, int pageSize = 10)
        {
            var query = _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.CreatedDate)
                .AsQueryable();

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResultDto<JobPost>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleJobStatus([FromBody] JobStatusToggleDto request)
        {
            var job = await _context.JobPosts.FindAsync(request.Id);
            if (job == null)
                return NotFound();

            job.IsActive = request.IsActive;
            _context.JobPosts.Update(job);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Job status updated successfully ✅", isActive = job.IsActive });
        }


        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var job = _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .FirstOrDefault(j => j.Id == id);

            if (job == null)
                return NotFound();

            return View(job);
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var job = _context.JobPosts.FirstOrDefault(j => j.Id == id);
            if (job == null)
                return NotFound();

            _context.JobPosts.Remove(job);
            _context.SaveChanges();

            TempData["Success"] = "Job deleted successfully!";
            return RedirectToAction("Jobs");
        }

        // -------------------- Companies --------------------
        [HttpGet]
        public IActionResult Companies(int page = 1, int pageSize = 10)
        {
            var query = _context.Companies
                .Include(c => c.JobPosts)
                .Include(c => c.EmployerProfiles)
                .OrderBy(c => c.Name)
                .AsQueryable();

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResultDto<Company>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(result);
        }

        // -------------------- Job Seekers --------------------
        [HttpGet]
        public IActionResult JobSeekers(int page = 1, int pageSize = 10)
        {
            var query = _context.JobSeekerProfiles
                .Include(j => j.Applications)
                .Include(i => i.CVFiles)
                .Include(i => i.SavedJobs)
                .OrderByDescending(j => j.CreatedDate)
                .AsQueryable();

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResultDto<JobSeekerProfile>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(result);
        }

        // -------------------- Industries --------------------
        [HttpGet]
        public IActionResult Industries(int page = 1, int pageSize = 10)
        {
            var query = _context.JobCategories
                .Include(i => i.JobPosts)
                .ThenInclude(i => i.Company)
                .AsQueryable();

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResultDto<JobCategory>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(result);
        }

        // -------------------- Jobs By Category --------------------
        [HttpGet]
        public IActionResult JobsByCategory(Guid id, int page = 1, int pageSize = 10)
        {
            var query = _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Where(j => j.JobCategoryId == id);

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var categoryName = _context.JobCategories
                .Where(c => c.Id == id)
                .Select(c => c.Name)
                .FirstOrDefault();

            ViewBag.CategoryName = categoryName;

            var result = new PagedResultDto<JobPost>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(result);
        }

        // -------------------- Companies By Category --------------------
        //[HttpGet]
        //public IActionResult CompaniesByCategory(Guid id, int page = 1, int pageSize = 10)
        //{
        //    var query = _context.Companies
        //        .Include(c => c.JobPosts)
        //        .Include(c => c.EmployerProfiles)
        //        .Where(c => c.JobCategoryId == id);

        //    var totalCount = query.Count();
        //    var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //    var categoryName = _context.JobCategories
        //        .Where(c => c.Id == id)
        //        .Select(c => c.Name)
        //        .FirstOrDefault();

        //    ViewBag.CategoryName = categoryName;

        //    var result = new PagedResultDto<Company>
        //    {
        //        Items = items,
        //        TotalCount = totalCount,
        //        PageNumber = page,
        //        PageSize = pageSize
        //    };

        //    return View(result);
        //}




        [HttpGet]
        public IActionResult CompanyDetails(Guid id)
        {
            var company = _context.Companies
                .Include(c => c.JobPosts)
                .Include(c => c.EmployerProfiles)
                .FirstOrDefault(c => c.Id == id);

            if (company == null)
                return NotFound();

            return View(company);
        }
        [HttpGet]
        public IActionResult EmployerDetails(Guid id)
        {
            var employer = _context.EmployerProfiles
                .Include(e => e.User)
                .Include(e => e.Company)
                .FirstOrDefault(e => e.Id == id);

            if (employer == null)
                return NotFound();

            return View(employer);
        }

        [HttpGet]
        public IActionResult JobCategoryDetails(Guid id)
        {
            var category = _context.JobCategories
                .Include(c => c.JobPosts)
                .ThenInclude(j => j.Company)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }
        [HttpGet]
        public IActionResult JobTypeDetails(Guid id)
        {
            var type = _context.JobTypes
                .Include(t => t.JobPosts)
                .ThenInclude(j => j.Company)
                .FirstOrDefault(t => t.Id == id);

            if (type == null)
                return NotFound();

            return View(type);
        }// -------------------- Job Types Management --------------------
        [HttpGet]
        public IActionResult CreateJobType()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobType(JobType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.JobTypes.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job type created successfully ✅";
            return RedirectToAction("Industries");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateJobType(Guid id)
        {
            var type = await _context.JobTypes.FindAsync(id);
            if (type == null)
                return NotFound();

            return View(type);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateJobType(JobType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.JobTypes.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job type updated successfully ✅";
            return RedirectToAction("Industries");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteJobType(Guid id)
        {
            var type = await _context.JobTypes.FindAsync(id);
            if (type == null)
                return NotFound();

            _context.JobTypes.Remove(type);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job type deleted successfully 🗑️";
            return RedirectToAction("Industries");
        }// -------------------- Job Categories Management --------------------
        [HttpGet]
        public IActionResult CreateJobCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobCategory(JobCategory model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.JobCategories.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job category created successfully ✅";
            return RedirectToAction("Industries");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateJobCategory(Guid id)
        {
            var category = await _context.JobCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateJobCategory(JobCategory model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.JobCategories.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job category updated successfully ✅";
            return RedirectToAction("Industries");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteJobCategory(Guid id)
        {
            var category = await _context.JobCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.JobCategories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job category deleted successfully 🗑️";
            return RedirectToAction("Industries");
        }



    }
}



//using AutoMapper;
//using DAL.DbContext;
//using Domains;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using NuGet.Packaging;

//namespace PortalSystemProject.Areas.admin.Controllers
//{
//    [Area("admin")]

//    public class DashboardController : Controller
//    {
//        private readonly PortalContext portalContext1;
//        private readonly IMapper mapper1;
//        public DashboardController(PortalContext portalContext,IMapper mapper)
//        {
//            portalContext1 = portalContext;
//            mapper1 = mapper;
//        }

//        [HttpGet]
//        public IActionResult Index(string period = "all")
//        {
//            var now = DateTime.UtcNow;
//            DateTime? fromDate = period switch
//            {
//                "week" => now.AddDays(-7),
//                "month" => now.AddMonths(-1),
//                _ => null
//            };

//             var jobs = portalContext1.JobPosts.AsQueryable();
//            if (fromDate.HasValue)
//                jobs = jobs.Where(j => j.CreatedDate >= fromDate.Value);

//            ViewBag.UserCount = portalContext1.Users.Count();
//            ViewBag.CompanyCount = portalContext1.Companies.Count();
//            ViewBag.JobCount = jobs.Count();
//            ViewBag.CategoryCount = portalContext1.JobCategories.Count();


//            ViewBag.CategoryLabels = portalContext1.JobCategories
//                .Select(c => c.Name)
//                .ToList();

//            ViewBag.CategoryJobCounts = portalContext1.JobCategories
//                .Select(c => jobs.Count(j => j.JobCategoryId == c.Id))
//                .ToList();


//            ViewBag.CompanyLabels = portalContext1.Companies
//                .Select(c => c.Name)
//                .Take(8)
//                .ToList();

//            ViewBag.CompanyJobCounts = portalContext1.Companies
//                .Select(c => jobs.Count(j => j.CompanyId == c.Id))
//                .Take(8)
//                .ToList();

//            ViewBag.SelectedPeriod = period;

//            return View();
//        }



//        [HttpGet]
//        public IActionResult GetDashboardData(string period = "all")
//        {
//            var now = DateTime.UtcNow;
//            DateTime? fromDate = period switch
//            {
//                "week" => now.AddDays(-7),
//                "month" => now.AddMonths(-1),
//                _ => null
//            };

//            var jobs = portalContext1.JobPosts.AsQueryable();
//            if (fromDate.HasValue)
//                jobs = jobs.Where(j => j.CreatedDate >= fromDate.Value);

//            var data = new
//            {
//                userCount = portalContext1.Users.Count(),
//                companyCount = portalContext1.Companies.Count(),
//                jobCount = jobs.Count(),
//                categoryCount = portalContext1.JobCategories.Count(),

//                categoryLabels = portalContext1.JobCategories.Select(c => c.Name).ToList(),
//                categoryJobCounts = portalContext1.JobCategories
//                    .Select(c => jobs.Count(j => j.JobCategoryId == c.Id))
//                    .ToList(),

//                companyLabels = portalContext1.Companies.Select(c => c.Name).Take(8).ToList(),
//                companyJobCounts = portalContext1.Companies
//                    .Select(c => jobs.Count(j => j.CompanyId == c.Id))
//                    .Take(8)
//                    .ToList()
//            };

//            return Json(data);
//        }


//        [HttpGet]
//        public IActionResult Employers()
//        {
//            var employers = portalContext1.EmployerProfiles.Include(d=> d.User).Include(d => d.Company).ToList();   
//            return View(employers);
//        }
//        [HttpGet]
//        public IActionResult Jobs()
//        {
//            var jobs = portalContext1.JobPosts
//                .Include(j => j.Company)
//                .Include(j => j.JobCategory)
//                .Include(j => j.JobType)
//                .Include(j => j.Applications)
//                .ToList();

//            return View(jobs);
//        }






//        [HttpGet]
//        public IActionResult Details(Guid id)
//        {
//            var job = portalContext1.JobPosts
//                .Include(j => j.Company)
//                .Include(j => j.JobCategory)
//                .Include(j => j.JobType)
//                .FirstOrDefault(j => j.Id == id);

//            if (job == null)
//                return NotFound();

//            return View(job);
//        }


//        [HttpGet]
//        public IActionResult Delete(Guid id)
//        {
//            var job = portalContext1.JobPosts.FirstOrDefault(j => j.Id == id);
//            if (job == null)
//                return NotFound();

//            portalContext1.JobPosts.Remove(job);
//            portalContext1.SaveChanges();

//            TempData["Success"] = "Job deleted successfully!";
//            return RedirectToAction("Jobs");
//        }

//        [HttpGet]
//        public IActionResult Users()
//        {
//            var users = portalContext1.Users.Include(u=>u.EmployerProfile).Include(u=>u.CreatedJobPosts).Include(u=>u.Applications).ToList();
//            return View(users); 
//        }

//        [HttpGet]
//        public IActionResult Companies()
//        {
//            var companies = portalContext1.Companies.Include(c=>c.JobPosts).Include(c=>c.EmployerProfiles).Include(c=>c.JobCategory).ToList();
//            return View(companies);
//        }
//        [HttpGet]
//        public IActionResult JobSeekers()
//        {
//            var jobSeekers = portalContext1.JobSeekerProfiles.Include(j=>j.Applications).Include(i=>i.CVFiles).Include(i=>i.SavedJobs)
//                .ToList();
//            return View(jobSeekers);
//        }
//        [HttpGet]
//        public IActionResult Industries()
//        {
//            var categories = portalContext1.JobCategories.Include(i=>i.JobPosts).ThenInclude(i=>i.Company).ToList();
//            return View(categories);
//        }
//        [HttpGet]
//        public IActionResult JobsByCategory(Guid id)
//        {
//            var jobs = portalContext1.JobPosts
//                .Include(j => j.Company)
//                .Include(j => j.JobCategory)
//                .Where(j => j.JobCategoryId == id)
//                .ToList();

//            var categoryName = portalContext1.JobCategories
//                .Where(c => c.Id == id)
//                .Select(c => c.Name)
//                .FirstOrDefault();

//            ViewBag.CategoryName = categoryName;

//            return View(jobs);
//        }
//        [HttpGet]
//        public IActionResult CompaniesByCategory(Guid id)
//        {
//            var companies = portalContext1.Companies
//                .Include(c => c.JobCategory)
//                .Include(c => c.EmployerProfiles)
//                .Include(c => c.JobPosts)
//                .Where(c => c.JobCategoryId == id)
//                .ToList();

//            var categoryName = portalContext1.JobCategories
//                .Where(c => c.Id == id)
//                .Select(c => c.Name)
//                .FirstOrDefault();

//            ViewBag.CategoryName = categoryName;

//            return View(companies);
//        }




//    }
//}
