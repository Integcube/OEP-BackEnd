using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceRecommendations
    {
        [Key]
        public int irId { get; set; }
        public string referenceNumber { get; set; }
        public string title { get; set; }
        public string significance { get; set; }
        public string type { get; set; }
        public int? priorityId { get; set; }
        public string insuranceRecommendation { get; set; }
        public string siteUpdates { get; set; }
        public string pcComments { get; set; }
        public int? insuranceStatusId { get; set; }
        public int? nomacStatusId { get; set; }
        public string latestStatus { get; set; }
        public DateTime? targetDate { get; set; }
        public string expectedBudget { get; set; }
        public int? proactiveId { get; set; }
        public int? siteId { get; set; }
        public int? sourceId { get; set; }
        public int? documentTypeId { get; set; }
        public string year { get; set; }
        public string report { get; set; }
        public int? recommendationTypeId { get; set; }
        public DateTime? createdDate { get; set; }
        public int? createdBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
