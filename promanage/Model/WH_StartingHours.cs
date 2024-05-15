using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class WH_StartingHours
    {
        [Key]
        public int startingId { get; set; }
        public int unitId { get; set; }
        public DateTime startDate { get; set; }
        public decimal startHours { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
        public decimal wceHours { get; set; }
    }
}
