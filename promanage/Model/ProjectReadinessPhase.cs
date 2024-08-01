using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("Project_ReadinessPhases")]
    public class ProjectReadinessPhase
    {
        [Key]

        public int PhaseId { get; set; }
        public string Title { get; set; }
        public decimal Weightage { get; set; }
        public int IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int planId { get; set; }
        public int displayOrder { get; set; }
        public int? ModifiedBy { get; set; }
       
        
    }
}
