using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("PreAssignInsuranceRecommendation")]
    public class PreAssignInsuranceRecommendation
    {
        [Key]
        public int AssignInsuranceId { get; set; }

        public int PreInsuranceId { get; set; }

        public int SiteId { get; set; }

        public int YearId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
