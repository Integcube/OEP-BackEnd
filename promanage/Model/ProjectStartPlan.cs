using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionTrakingSystem.Model
{
    [Table("Project_StartPlan")]
    public class ProjectStartPlan
    {
        [Key]
        public int id { get; set; }
        public int planId { get; set; }
        public int siteId { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
        public int pmId { get; set; }

        
    }
}
