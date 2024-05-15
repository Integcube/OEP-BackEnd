using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_PhaseDuration
    {
        [Key]
        public int phaseDurId { get; set; }
        public int phaseId { get; set; }
        public int outageId { get; set; }
        public decimal durationMonths { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
