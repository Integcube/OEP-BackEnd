using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILActionTracker
    {
        [Key]
        public int tilActionTrackerId { get; set; }
        public int tapId { get; set; }
        public int siteEquipmentId { get; set; }
        public int statusCalculated { get; set; }
        public string siteStatusDetail { get; set; }
        public int? budgetId { get; set; }
        public int? partsServiceId { get; set; }
        public int? sapPlanningId { get; set; }
        public int? finalImplementationId { get; set; }
        public DateTime targetDate { get; set; }
        public string calStatus { get; set; }
        public decimal? calcPriority { get; set; }
        public string budgetCalc { get; set; }
        public string ddtCalc { get; set; }
        public string evidenceCalc { get; set; }
        public string implementationCalc { get; set; }
        public string partsCalc { get; set; }
        public string sapCalc { get; set; }
        public int? evidenceId { get; set; }
        public int? assignedTo { get; set; }
        public int? isDeleted { get; set; }
        public DateTime? createdOn { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? createdBy { get; set; }
        public int? modifiedBy { get; set; }
        public string adminComment { get; set; }
        public string reviewerComment { get; set; }
        public int isCompleted { get; set; }
        public int rework { get; set; }
        public int clusterReviewed { get; set; }
        public DateTime? implementedDate { get; set; }
        public DateTime? actionClosureDate { get; set; }
        public int? actionClosedBy { get; set; }
    }
}
