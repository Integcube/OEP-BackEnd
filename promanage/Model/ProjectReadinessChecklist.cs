using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("Project_ReadinessChecklist")]
    public class ProjectReadinessChecklist
    {
        [Key]
        public int TaskId { get; set; }

        public string Title { get; set; }

        public int ParentId { get; set; }

        public decimal IdealScore { get; set; }

        public decimal ActualScore { get; set; }

        public int Responsibility { get; set; }

        public DateTime? TargetDate { get; set; }

        public string Remarks { get; set; }

        public int PhaseId { get; set; }
        public int PlanId { get; set; }

        public int IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
