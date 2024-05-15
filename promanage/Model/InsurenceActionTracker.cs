using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsurenceActionTracker
    {
        [Key]
        public int iaId { get; set; }
        public int? recommendationId { get; set; }
        public int? assignedTo { get; set; }
        public string action { get; set; }
        public string assignerComment { get; set; }
        public DateTime targetDate { get; set; }
        public int? iatStatusId { get; set; }
        public int? iatCompanyId { get; set; }
        public string comments { get; set; }
        public DateTime? closureDate { get; set; }
        public int? evidenceAvailableId { get; set; }
        public decimal? calcStatus { get; set; }
        public decimal? calcEvid { get; set; }
        public decimal? calcDate { get; set; }
        public string completionScore { get; set; }
        public string daysToTarget { get; set; }
        public string scoreDetails { get; set; }
        public int? sourceId { get; set; }
        public int isDeleted { get; set; }
        public DateTime createdOn { get; set; }
        public int? createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int? dayStatus { get; set; }
        public string adminComment { get; set; }
        public string reviewerComment { get; set; }
        public int isCompleted { get; set; }
        public int rework { get; set; }
        public int clusterReviewed { get; set; }
        public DateTime? implementedDate { get; set; }
        public int? actionClosedBy { get; set; }
    }
}
