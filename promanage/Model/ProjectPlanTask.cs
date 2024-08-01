using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionTrakingSystem.Model
{
    [Table("Project_PlanTask")]
    public class ProjectPlanTask
    {
        [Key]
        public int taskId { get; set; }
        public string title { get; set; }
        public decimal duration { get; set; }
        public decimal weightage { get; set; }
        public int planId { get; set; }
        public int phaseId { get; set; }
        public int taskParentId { get; set; }
        public decimal lagDays { get; set; }
        public int predecessorId { get; set; }
        public DateTime? createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public string code { get; set; }
        public int predecessorType { get; set; }
        public decimal idealScore { get; set; }
        public int durationUnit { get; set; }
        public int lagUnit { get; set; }
    }
}
