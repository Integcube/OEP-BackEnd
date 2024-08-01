using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionTrakingSystem.Model
{
    [Table("Project_Plan")]
    public class ProjectPlan
    {
        [Key]
        public int planId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
    }
}
