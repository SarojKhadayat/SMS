using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using SMS.Common;
using SMS.Models.DbModel;
using SMS.Models.Repository;
using SMS.Models.ViewModel;

namespace SMS.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        [SessionStateFilter]
        public ActionResult Index()
        {
            if (Session["email"]==null)
            {
                return RedirectToAction("Login", "Account");
            }
            StudentRepository studentRepository = new StudentRepository();
            var students = studentRepository.GetAllStudents();
            return View(students);
        }

        public ActionResult Create()
        {
            SchoolManagementSystemEntities _db = new SchoolManagementSystemEntities();
            StudentRepository studentRepository = new StudentRepository();
            StudentViewModel students = new StudentViewModel();
            ViewBag.facultyList = (from faculty in _db.TblFaculties
                                   select new SelectListItem
                                   {
                                       Text = faculty.FacultyName + "-" + faculty.Section,
                                       Value = faculty.Id.ToString()
                                   });
            students.Subjects = studentRepository.GetAllSubjects();
            return View(students);
        }
        [HttpPost]
        public ActionResult Create(StudentViewModel student)
        {
            StudentRepository repo = new StudentRepository();
            repo.InsertStudent(student);
           // repo.InsertIntoStudentUsingAdo(student);
            return Json(true);
        }

        public ActionResult Edit(int id)
        {
            StudentRepository repo = new StudentRepository();
            SchoolManagementSystemEntities _db = new SchoolManagementSystemEntities();
            ViewBag.facultyList = (from faculty in _db.TblFaculties
                                   select new SelectListItem
                                   {
                                       Text = faculty.FacultyName + "-" + faculty.Section,
                                       Value = faculty.Id.ToString()
                                   });
            var student = repo.GetStudentById(id);
            return View(student);
        }
        [HttpPost]
        public ActionResult Edit(StudentViewModel studentViewModel)
        {
            StudentRepository repository=new StudentRepository();
            repository.UpdateStudent(studentViewModel);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            StudentRepository repo=new StudentRepository();
            repo.DeleteStudent(id);
            return RedirectToAction("Index");
        }
        public ActionResult IndexAjax()
        {
            return View();
        }

        public ActionResult _IndexPartial()
        {
            StudentRepository studentRepository = new StudentRepository();
            var students = studentRepository.GetAllStudents();
            return View(students);
        }
        public ActionResult _Create()
        {
            SchoolManagementSystemEntities _db = new SchoolManagementSystemEntities();
            StudentRepository studentRepository = new StudentRepository();
            StudentViewModel students = new StudentViewModel();
            ViewBag.facultyList = (from faculty in _db.TblFaculties
                select new SelectListItem
                {
                    Text = faculty.FacultyName + "-" + faculty.Section,
                    Value = faculty.Id.ToString()
                });
            students.Subjects = studentRepository.GetAllSubjects();
            return View(students);
        }
        public ActionResult DownloadReport()
        {
            LocalReport lr = new LocalReport();
            StudentRepository repository=new StudentRepository();
            string path = Path.Combine(Server.MapPath("~/Views/Reports"), "StudentDetails.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {

            }

            var students = repository.GetAllStudents();
            ReportDataSource rd = new ReportDataSource("DataSet1", students);
            lr.DataSources.Add(rd);
            IList<ReportParameter> reportParameters = new List<ReportParameter>()
            {
                new ReportParameter("title", "Student Detail Report"),
                new ReportParameter("footer", "Generatd on" + DateTime.Now)
            };
            lr.SetParameters(reportParameters);
            string mimeType;
            string encoding;
            string fileNameExtension;
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            lr.Refresh();
            renderedBytes = lr.Render(
                "EXCEL",
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            Response.AddHeader("Content-Disposition",
                "attachment; filename=Test.xls");

            return new FileContentResult(renderedBytes, mimeType);
        }
    }
}