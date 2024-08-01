using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("Project_TaskAssignment")]
    public class ProjectTaskAssignment
    {
        [Key]
        public int TaskAssignmentId { get; set; }

        public int TaskId { get; set; }

        [MaxLength(10)]
        public int StartplanId { get; set; }

        public int AssignTo { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Weightage { get; set; }

        public decimal Duration { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Progress { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int IsDeleted { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
