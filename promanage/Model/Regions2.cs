using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    public class Regions2
    {
        [Key]
        public int regionId { get; set; }
        public string title { get; set; }
        public string TILsSummary { get; set; }
        public string insuranceSummary { get; set; }
        public DateTime createdOn { get; set; }
        public int? createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int? executiveDirector { get; set; }
        public int isDeleted { get; set; }
    }
}
