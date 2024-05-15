using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_PhaseOutageTracker
    {
        [Key]
        public int potId { get; set; }
        public int equipmentId { get; set; }
        public int phaseId { get; set; }
        public int phaseReadId { get; set; }
        public DateTime outageDate { get; set; }
        public int notApplicable { get; set; }
        public int statusId { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
        public int? snoId { get; set; }
    }
}
