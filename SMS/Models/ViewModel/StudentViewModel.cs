using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using SMS.Models.DbModel;

namespace SMS.Models.ViewModel
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This is required")]
        [DisplayName("Name")]
        public string Name { get; set; }
        public string Address { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Date Of Birth")]
        public Nullable<DateTime> DateOfBirth { get; set; }
        [DisplayName("Father Name")]
        public string FatherName { get; set; }
        public Nullable<int> FacultyId { get; set; }
        public string FacultyName { get; set; }
        public string Section { get; set; }
        public string Gender { get; set; }
        public List<SubjectSelectViewModel> Subjects { get; set; }
        public string SubjectNames { get; set; }
    }
}