using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_PhaseReadinessDescription
    {
        [Key]
        public int phaseReadId { get; set; }
        public int phaseId { get; set; }
        public string phaseReadDesc { get; set; }
        public int phaseReadNum { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
