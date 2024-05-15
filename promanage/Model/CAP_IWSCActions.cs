using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_IWSCActions
    {
        [Key]
        public int wscActionId { get; set; }
        public int? obsId { get; set; }
        public string action { get; set; }
        public int? priorityId { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? targetDate { get; set; }
        public DateTime? closureDate { get; set; }
        public string remarks { get; set; }
        public int? deptId { get; set; }
        public int? userId { get; set; }
        public int? statusId { get; set; }
        public string referenceNumber { get; set; }
        public string suggestion { get; set; }
        public DateTime? createdDate { get; set; }
        public int? createdBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
