using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem
{
    public class QF_User
    {
        [Key]
        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public DateTime createdOn { get; set; }
    }
}
