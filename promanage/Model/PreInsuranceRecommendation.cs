using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("PreInsuranceRecommendation")]
    public class PreInsuranceRecommendation
    {
        [Key]
        public int PreInsuranceId { get; set; }
        public string Title { get; set; }
        public int TechId { get; set; }
        public int IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
