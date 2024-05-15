using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_SiteNextOutages
    {
        [Key]
        public int snoId { get; set; }
        public int equipmentId { get; set; }
        public int outageId { get; set; }
        public DateTime nextOutageDate { get; set; }
        public decimal? outageDurationInDays { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
