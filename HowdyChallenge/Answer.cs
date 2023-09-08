using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowdyChallenge
{
    /// <summary>
    /// Data model for employee answer
    /// </summary>
    public class Answer
    {
        public int EmployeeId { get; set; }
        public int GroupId { get; set; }
        public DateTime AnsweredOn { get; set; }
        public byte Answer1 { get; set; }
        public byte Answer2 { get; set; }
        public byte Answer3 { get; set; }
        public byte Answer4 { get; set; }
        public byte Answer5 { get; set; }
    }
}
