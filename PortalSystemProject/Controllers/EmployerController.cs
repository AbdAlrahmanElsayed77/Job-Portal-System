using BL.Contracts;
using BL.Dtos;
using BL.Services;
using Domains;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
using System.Drawing;
using System.Security.Claims;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace PortalSystemProject.Controllers
{
    public class EmployerController : Controller
    {
        public EmployerController
            (
            IJobPostRepository jobPostService,
            IEmployerProfileService employerProfileService,
            ICompanyRepository companyService,
            IJobCategoryRepository categoryService,
            IJobTypeRepository jobTypeService,
            IApplicationRepository applicationService
            )
        {
            JobPostService = jobPostService;
            EmployerProfileService = employerProfileService;
            CompanyService = companyService;
            CategoryService = categoryService;
            JobTypeService = jobTypeService;
            ApplicationService = applicationService;
            //EmpId = GetCurrentEmployerId();
            //EmpId = GetCurrentUserId();
        }

        public IJobPostRepository JobPostService { get; }
        public IEmployerProfileService EmployerProfileService { get; }
        public ICompanyRepository CompanyService { get; }
        public IJobCategoryRepository CategoryService { get; }
        public IJobTypeRepository JobTypeService { get; }
        public IApplicationRepository ApplicationService { get; }

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

        //create a jobpost
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
            JobPostService.Update(model,GetCurrentUserId());
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

        //details
        public IActionResult Details(Guid id)
        {
            var job = JobPostService.GetById(id);
            if (job == null)
                return NotFound();

            return View(job);
        }
        /// /////////////////////////////////////////////////Employers applications /////////////////////////////////
        // View applicants per job
        public IActionResult Applications(Guid id)
        {
            var job = JobPostService.GetById(id);
            ViewBag.jobPost = id;
            if (job == null)
                return NotFound();

            ViewBag.JobTitle = job.Title;
            ViewBag.JobPostId = job.Id;
            var applications = ApplicationService.GetApplicationsByJob(id);
            return View(applications);
        }

        //  Update application status
        [HttpPost]
        public IActionResult UpdateApplicationStatus(Guid id, Status status)
        {
            ApplicationService.UpdateStatus(id, status);
            var app = ApplicationService.GetById(id);
            return RedirectToAction(nameof(Applications), new { id = app!.JobPostId });
        }

        //download applicants list as pdf and excel
        public IActionResult ExportToExcel(Guid jobPostId)
        {
            var applications = ApplicationService.GetApplicationsByJob(jobPostId).ToList();
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ExcelPackage.License.SetNonCommercialOrganization("Job Portal");


            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Applications");
                sheet.Cells["A1"].Value = "Applicant Name";
                sheet.Cells["B1"].Value = "Status";
                sheet.Cells["C1"].Value = "Applied At";
                sheet.Cells["D1"].Value = "Cover Letter";

                using (var range = sheet.Cells["A1:D1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                int row = 2;
                foreach (var app in applications)
                {
                    sheet.Cells[row, 1].Value = app.ApplicantName;
                    sheet.Cells[row, 2].Value = app.Status.ToString();
                    sheet.Cells[row, 3].Value = app.AppliedAt.ToString("yyyy-MM-dd");
                    sheet.Cells[row, 4].Value = app.CoverLetter ?? "—";
                    row++;
                }

                sheet.Cells.AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                string fileName = $"Applications_{DateTime.Now:yyyyMMddHHmm}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // ✅ Export to PDF
        public IActionResult ExportToPdf(Guid jobPostId)
       {
            var applications = ApplicationService.GetApplicationsByJob(jobPostId).ToList();

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 25, 25);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                document.Add(new Paragraph("Applicants Report", titleFont));
                document.Add(new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}\n\n", textFont));

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 1f, 1.5f, 3f });

                // Headers
                string[] headers = { "Applicant Name", "Status", "Applied At", "Cover Letter" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont))
                    {
                        BackgroundColor = new BaseColor(230, 230, 230),
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                // Data rows
                foreach (var app in applications)
                {
                    table.AddCell(new Phrase(app.ApplicantName, textFont));
                    table.AddCell(new Phrase(app.Status.ToString(), textFont));
                    table.AddCell(new Phrase(app.AppliedAt.ToString("yyyy-MM-dd"), textFont));
                    table.AddCell(new Phrase(app.CoverLetter ?? "—", textFont));
                }

                document.Add(table);
                document.Close();

                string fileName = $"Applications_{DateTime.Now:yyyyMMddHHmm}.pdf";
                return File(stream.ToArray(), "application/pdf", fileName);
            }
        }

        public IActionResult Dashboard()
        {
            // Fetch employer-related data (replace with current logged-in employer)
            Guid employerId = GetCurrentUserId(); // temporary for test

            var jobPosts = JobPostService.GetAll().Where(j => j.CreatedByUserId == employerId);
            var applications = ApplicationService.GetAll()
                .Where(a => jobPosts.Select(j => j.Id).Contains(a.JobPostId));

            // Analytics
            var totalJobs = jobPosts.Count();
            var totalApplicants = applications.Count();
            var acceptedCount = applications.Count(a => a.Status == Domains.Status.Accepted);
            var rejectedCount = applications.Count(a => a.Status == Domains.Status.Rejected);
            var activeJobs = jobPosts.Count(j => j.IsActive);
            var expiredJobs = jobPosts.Count(j => j.ExpiresAt.HasValue && j.ExpiresAt < DateTime.Now);

            var dashboardData = new
            {
                totalJobs,
                totalApplicants,
                acceptedCount,
                rejectedCount,
                activeJobs,
                expiredJobs
            };

            ViewBag.DashboardData = dashboardData;
            return View();
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
