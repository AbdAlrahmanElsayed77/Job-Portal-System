using BL.Contracts;
using BL.Dtos;
using Microsoft.AspNetCore.Mvc;
using PortalSystemProject.Models;
using System.Diagnostics;
using System.Net.WebSockets;

namespace PortalSystemProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJobTypeRepository _jopTYPE;

        public HomeController(ILogger<HomeController> logger , IJobTypeRepository jopTYPE)
        {
            _logger = logger;
            _jopTYPE = jopTYPE;
        }

        public IActionResult AllJops()
        {
            var result= _jopTYPE.GetAll();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
           
            return View();
        }
        [HttpPost]
        public IActionResult Create(JobTypeDto jobType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _jopTYPE.Add(jobType);
                    return RedirectToAction("AllJops");
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            
            return View();
        }

        public IActionResult Delete(Guid id) 
        {
            try
            {
             _jopTYPE.ChangeStatus(id);
                return RedirectToAction("AllJops");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




    }
}
