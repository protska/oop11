using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using oop11.MVVM.View;
using oop4_5.Core;

namespace oop11.MVVM.Model
{

    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Speciality { get; set; }
        public int sGroup { get; set; }
        public int Subgroup { get; set; }
        public int Course { get; set; }
        public ICommand Command { get; set; }


        public Student(int id, string fullName, string speciality, int group, int subgroup, int course)
        {
            Id = id;
            FullName = fullName;
            Speciality = speciality;
            sGroup = group;
            Subgroup = subgroup;
            Course = course;
        }
    }

}
