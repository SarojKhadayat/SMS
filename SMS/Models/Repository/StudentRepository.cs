using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SMS.Controllers;
using SMS.Models.DbModel;
using SMS.Models.ViewModel;

namespace SMS.Models.Repository
{
    public class StudentRepository
    {
        private readonly SchoolManagementSystemEntities _db = new SchoolManagementSystemEntities();
        public List<StudentViewModel> GetAllStudents()
        {
            //  List<tblStudent> students=new List<tblStudent>();
            //LINQ
            // students = (from stud in _db.tblStudents select stud).ToList();
            //select name from tblStudent
            //Lamda Expression
            //var test= _db.tblStudents.Select(x => x).ToList();
            List<StudentViewModel> studentViewModel = (from stud in _db.tblStudents
                                                       select new StudentViewModel()
                                                       {
                                                           Id = stud.Id,
                                                           Name = stud.Name,
                                                           Address = stud.Address,
                                                           PhoneNumber = stud.PhoneNumber,
                                                           FatherName = stud.FatherName,
                                                           FacultyId = stud.FacultyId,
                                                           FacultyName = stud.TblFaculty.FacultyName,
                                                           Section = stud.TblFaculty.Section,
                                                           Gender = stud.Gender,
                                                           DateOfBirth = stud.DateOfBirth
                                                       }).ToList();
            foreach (var student in studentViewModel)
            {
                student.SubjectNames = GetSubjectsById(student.Id);
            }
            return studentViewModel;
        }

        public List<SubjectSelectViewModel> GetAllSubjects()
        {
            var subjects = (from subject in _db.tblSubjects
                            select new SubjectSelectViewModel()
                            {
                                Id = subject.Id,
                                SubjectName = subject.Name
                            }).ToList();
            return subjects;
        }

        public bool InsertStudent(StudentViewModel studentVm)
        {
            tblStudent student = new tblStudent()
            {
                Name = studentVm.Name,
                Address = studentVm.Address,
                DateOfBirth = studentVm.DateOfBirth,
                Gender = studentVm.Gender,
                FacultyId = studentVm.FacultyId,
                FatherName = studentVm.FatherName,
                PhoneNumber = studentVm.PhoneNumber
            };
            _db.tblStudents.Add(student);
            int id = _db.SaveChanges();
            if (id > 0)
            {
                List<int> subjectIds = studentVm.Subjects.Where(x => x.IsSelected == true).Select(x => x.Id).ToList();
                InsertStudentSubjects(subjectIds, student.Id);
            }
            return true;
        }

        public bool InsertStudentSubjects(List<int> subjectIds, int studentId)
        {
            bool status = false;
            foreach (var subjectId in subjectIds)
            {
                tblStudentSubject studentSubject = new tblStudentSubject()
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                };
                _db.tblStudentSubjects.Add(studentSubject);
                _db.SaveChanges();
                status = true;
            }
            return status;
        }

        public string GetSubjectsById(int studentId)
        {
            List<string> subjectNames = _db.tblStudentSubjects.Where(x => x.StudentId == studentId)
                .Select(x => x.tblSubject.Name).ToList();
            return string.Join(",", subjectNames);
        }

        public StudentViewModel GetStudentById(int id)
        {
            var student = (from stud in _db.tblStudents
                           where stud.Id == id
                           select new StudentViewModel()
                           {
                               Id = stud.Id,
                               Name = stud.Name,
                               Address = stud.Address,
                               PhoneNumber = stud.PhoneNumber,
                               FatherName = stud.FatherName,
                               FacultyId = stud.FacultyId,
                               FacultyName = stud.TblFaculty.FacultyName,
                               Section = stud.TblFaculty.Section,
                               Gender = stud.Gender,
                               DateOfBirth = stud.DateOfBirth,
                           }).FirstOrDefault();
            if (student != null)
                student.Subjects = GetAssignedSubjectsById(id);
            return student;
        }

        public List<SubjectSelectViewModel> GetAssignedSubjectsById(int studentId)
        {
            List<SubjectSelectViewModel> subjects = GetAllSubjects();
            List<int?> selectedIds = _db.tblStudentSubjects.Where(x => x.StudentId == studentId)
                .Select(x => x.SubjectId).ToList();
            foreach (var id in selectedIds)
            {
                foreach (var subject in subjects)
                {
                    if (subject.Id == id)
                    {
                        subject.IsSelected = true;
                    }
                }
            }
            return subjects;
        }

        public bool UpdateStudent(StudentViewModel student)
        {
            tblStudent oldStudent = _db.tblStudents.Find(student.Id);
            tblStudent newStudent = new tblStudent
            {
                Id = student.Id,
                Address = student.Address,
                Name = student.Name,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                FacultyId = student.FacultyId,
                FatherName = student.FatherName,
                PhoneNumber = student.PhoneNumber
            };
            _db.Entry(oldStudent).CurrentValues.SetValues(newStudent);
            _db.SaveChanges();
            UpdateStudentSubjects(student.Subjects, student.Id);
            return _db.SaveChanges() > 0;

        }

        public bool UpdateStudentSubjects(List<SubjectSelectViewModel> subjectSelectViewModels, int studentId)
        {
            List<int> selectedValues =
                subjectSelectViewModels.Where(x => x.IsSelected == true).Select(x => x.Id).ToList();
            List<int> deselectedValues =
                subjectSelectViewModels.Where(x => x.IsSelected == false).Select(x => x.Id).ToList();
            foreach (var id in selectedValues)
            {
                var subject = (from substud in _db.tblStudentSubjects
                               where substud.StudentId == studentId && substud.SubjectId == id
                               select substud).FirstOrDefault();
                if (subject == null)
                {
                    tblStudentSubject studentSubject = new tblStudentSubject()
                    {
                        StudentId = studentId,
                        SubjectId = id
                    };
                    _db.tblStudentSubjects.Add(studentSubject);
                }

            }

            foreach (var subjectId in deselectedValues)
            {
                var subject = (from substud in _db.tblStudentSubjects
                               where substud.StudentId == studentId && substud.SubjectId == subjectId
                               select substud).FirstOrDefault();
                if (subject != null)
                {
                    _db.tblStudentSubjects.Remove(subject);
                    _db.SaveChanges();
                }
            }

            return true;

        }

        public bool DeleteStudent(int id)
        {
            tblStudent student = _db.tblStudents.Find(id);
            List<tblStudentSubject> studentSubjects =
                (from ss in _db.tblStudentSubjects where ss.StudentId == id select ss).ToList();
            foreach (var studentSubject in studentSubjects)
            {
                _db.tblStudentSubjects.Attach(studentSubject);
                _db.tblStudentSubjects.Remove(studentSubject);
            }
            if (student != null)
            {
                _db.tblStudents.Attach(student);
                _db.tblStudents.Remove(student);
            }
            return _db.SaveChanges() > 0;
        }

        #region ADO.NET
        public void InsertIntoStudentUsingAdo(StudentViewModel student)
        {
            string connectionString = System.Configuration.ConfigurationManager.
                ConnectionStrings["SchoolManagementSystem"].ConnectionString;
            string test = System.Configuration.ConfigurationManager.AppSettings["Name"];
            string query = "Insert into tblStudent (Name,Address,PhoneNumber,FatherName,FacultyId,Gender," +
                           "DateOfBirth)" +
                           "values (@Name,@Address,@PhoneNumber,@FatherName,@FacultyId,@Gender,@DateOfBirth)";
            SqlConnection cn = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@Address", student.Address);
                cmd.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                cmd.Parameters.AddWithValue("@FatherName", student.FatherName);
                cmd.Parameters.AddWithValue("@FacultyId", student.FacultyId);
                cmd.Parameters.AddWithValue("@Gender", student.Gender);
                cmd.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public List<StudentViewModel> GetAllStudentsAdo()
        {
            string connectionString = System.Configuration.ConfigurationManager.
                ConnectionStrings["SchoolManagementSystem"].ConnectionString;
            string query = "select * from tblStudent";
            List<StudentViewModel> students = new List<StudentViewModel>();
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    StudentViewModel student = new StudentViewModel();
                    student.Id = Convert.ToInt32(reader["Id"]);
                    student.Name = reader["Name"].ToString();
                    student.Address = reader["Address"].ToString();
                    student.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                    student.FacultyId = Convert.ToInt32(reader["FacultyId"]?.ToString());
                    student.FatherName = reader["FatherName"].ToString();
                    student.Gender = reader["Gender"].ToString();
                    students.Add(student);
                }
                con.Close();
            }

            return students;
        }
        public void InsertStudentUsingSp(StudentViewModel student)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SchoolManagementSystem"].ConnectionString;
            string cmdText = "InsertIntoStudent";
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(cmdText, con))
            {
                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar));
                cmd.Parameters["@Name"].Value = student.Name;
                cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.VarChar));
                cmd.Parameters["@Address"].Value = student.Address;

                cmd.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.VarChar));
                cmd.Parameters["@PhoneNumber"].Value = student.PhoneNumber;

                cmd.Parameters.Add(new SqlParameter("@FatherName", SqlDbType.Int));
                cmd.Parameters["@FatherName"].Value = student.FatherName;

               // cmd.Parameters["@PId"].Value = student.Id;
                //   cmd.Parameters.Add("@PId", SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
                //int retval = (int)cmd.Parameters["@PId"].Value;
                con.Close();
            }
        }
        #endregion
    }
}