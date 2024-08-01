using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionTrakingSystem.Model
{
    [Table("Project_Phase")]
    public class ProjectPhase
    {
        [Key]
        public int phaseId { get; set; }
        public string title { get; set; }
        public int planId { get; set; }
        public decimal weightage { get; set; }
        public int duration { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
         public int displayOrder { get; set; }

    }
}
