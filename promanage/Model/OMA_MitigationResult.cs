using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_MitigationResult
    {
        [Key]
        public int resultId { get; set; }
        public int actionId { get; set; }
        public int siteId { get; set; }
        public int technologyId { get; set; }
        public int tataInvolvement { get; set; }
        public string thirdPartyInterface { get; set; }
        public int? statusId { get; set; }
        public string reviewerComment { get; set; }
        public int isReviewed { get; set; }
        public string actionComment { get; set; }
        public int? rework { get; set; }
        public DateTime? targetDate { get; set; }
    }
}
