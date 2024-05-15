using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class Country
    {
        [Key]
        public int countryId { get; set; }
        public int? clustedId { get; set; }
        public int regionId { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public int? executiveDirector { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }

    }
}
