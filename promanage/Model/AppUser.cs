using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class AppUser
    {
        [Key]
        public int userId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public int? lastLoginId { get; set; }
        public int isDeleted { get; set; }
        public string picPath { get; set; }
        public string cell { get; set; }

        public DateTime? createdOn { get; set; }
        public int? createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
    }
}
