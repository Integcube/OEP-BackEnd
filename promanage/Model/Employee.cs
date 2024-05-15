using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class Employee
    {
        [Key]
        public int empId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public int userId { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public string about { get; set; }
        public string cell { get; set; }
        public string address { get; set; }
        public string education { get; set; }
        public string experience { get; set; }
        public string workshops { get; set; }
        public int isDeleted { get; set; }
        public string mobile { get; set; }
        public string gender { get; set; }
        public DateTime DOB { get; set; }
        public string designation { get; set; }
        public string city { get; set; }
        public string country { get; set; }
    }
}
