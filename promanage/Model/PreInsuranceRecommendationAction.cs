using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("PreInsuranceRecommendationAction")]
    public class PreInsuranceRecommendationAction
    {
        [Key]
        public int PreInsuranceActionId { get; set; }
        public int PreInsuranceId { get; set; }
        public string Title { get; set; }
        public string EvidenceType { get; set; }
        public string Guidelines { get; set; }
        public string AdditionalGuidelines { get; set; }
        public int IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
