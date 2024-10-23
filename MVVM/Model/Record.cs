using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oop11.MVVM.Model
{
    internal class Record
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string Date { get; set; }
        public string Score {  get; set; }
        public string Skip { get; set; }

        public Record(int id, string studentName, string date, string score, string skip)
        {
            Id = id;
            StudentName = studentName;
            Date = date;
            Score = score;
            Skip= skip;
        }
    }
}
