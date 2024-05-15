using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_PhaseOutageTrackerProgress
    {
        [Key]
        public int progressId { get; set; }
        public int potId { get; set; }
        public int monthId { get; set; }
        public int yearId { get; set; }
        public decimal? progress { get; set; }
        public string remarks { get; set; }
        public int createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
    }
}
