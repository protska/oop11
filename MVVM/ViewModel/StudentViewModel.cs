using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using oop11.MVVM.Model;
using oop4_5.Core;

namespace oop11.MVVM.ViewModel
{

    internal class StudentViewModel : ObservableObject
    {
        public Student Student;
        public StudentViewModel(Student student)
        {
            this.Student = student;
        }

        public string FullName
        {
            get
            {
                return this.Student.FullName;
            }
            set
            {
                this.Student.FullName = value;
                OnPropertyChanged("Name");
            }
        }

        public string Speciality
        {
            get
            {
                return this.Student.Speciality;
            }
            set
            {
                this.Student.Speciality = value;
                OnPropertyChanged("Speciality");
            }
        }

        public int sGroup
        {
            get
            {
                return this.Student.sGroup;
            }
            set
            {
                this.Student.sGroup = value;
                OnPropertyChanged("sGroup");
            }
        }

        public int Subgroup
        {
            get
            {
                return this.Student.Subgroup;
            }
            set
            {
                this.Student.Subgroup = value;
                OnPropertyChanged("Subgroup");
            }
        }

        public int Course
        {
            get
            {
                return this.Student.Course;
            }
            set
            {
                this.Student.Course = value;
                OnPropertyChanged("Course");
            }
        }
    }
}
