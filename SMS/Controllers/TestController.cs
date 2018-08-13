using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models.DbModel;
using SMS.Models.ViewModel;

namespace SMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(string studentName, string studentAddress, string dob)
        {
            ViewBag.studentName = String.IsNullOrEmpty(studentName) ? " " : studentName;
            ViewBag.studentAddress = String.IsNullOrEmpty(studentAddress) ? " " : studentAddress;
            ViewBag.dob = String.IsNullOrEmpty(dob) ? " " : dob;
            return View();
        }

        public ActionResult Create2()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create2(StudentViewModel student)
        {
            if (ModelState.IsValid)            
            return RedirectToAction("Index");
            return View();
        }
    }
}