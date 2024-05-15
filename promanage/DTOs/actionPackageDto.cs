using Microsoft.AspNetCore.Http;
using System;

namespace ActionTrakingSystem.DTOs
{
    public class filterEqTap
    {
        public int userId { get; set; }
        public int siteId { get; set; }
    }
    public class packageActionUserDto
    {
        public int userId { get; set; }
        public int packageId { get; set; }


    }
    public class tilFileEnduser
    {
        public IFormFile tilReport { get; set; }
        public string tatId { get; set; }
        public string userId { get; set; }


    }
    public class tilPackagetrackeruser
    {
        public int userId { get; set; }
        public tilActionTrackerackageDto action { get; set; }
    }
    public class tilActionTrackerackageDto
    {
        public int tilActionTrackerId { get; set; }
        public int? tapId { get; set; }
        public string tilAction { get; set; }
        public int? assignedToId { get; set; }
        public string assignedToTitle { get; set; }
        public string actionDescription { get; set; }
        public int? siteEquipmentId { get; set; }
        public string siteEquipmentTitle { get; set; }
        public int? userId { get; set; }
        public string userName { get; set; }
        public int? budgetSourceId { get; set; }
        public string budgetSourceTitle { get; set; }
        public int? statusCalculated { get; set; }
        public string siteStatusDetail { get; set; }
        public int? budgetId { get; set; }
        public string budgetTitle { get; set; }
        public int? partServiceId { get; set; }
        public string partServiceTitle { get; set; }
        public int? planningId { get; set; }
        public string planningTitle { get; set; }
        public string finalImplementationTitle { get; set; }
        public int? finalImplementationId { get; set; }
        public DateTime targetDate { get; set; }
        public int? priorityId { get; set; }
        public string priorityTitle { get; set; }
        public string calStatus { get; set; }
        public decimal? calcPriority { get; set; }
        public string budgetCalc { get; set; }
        public string ddtCalc { get; set; }
        public string evidenceCalc { get; set; }
        public string implementationCalc { get; set; }
        public string partsCalc { get; set; }
        public string sapCalc { get; set; }
        public int? evidenceId { get; set; }
        public string evidenceTitle { get; set; }
        public string adminComment { get; set; }
        public string reviewerComment { get; set; }
        public bool? isCompleted { get; set; }
        public bool? rework { get; set; }
        public bool? clusterReviewed { get; set; }
    }
    public class actionPackageDto
    {
        public int? packageId { get; set; }
        public int? tilId { get; set; }
        public string tilNumber { get; set; }
        public string actionTitle { get; set; }
        public int? actionClosureGuidelinesId { get; set; }
        public string actionCategory { get; set; }
        public int? outageId { get; set; }
        public string unitStatus { get; set; }
        public string actionDescription { get; set; }
        public string expectedBudget { get; set; }
        public int? budgetSourceId { get; set; }
        public string budegetSource { get; set; }
        public string patComments { get; set; }
        public int? priorityId { get; set; }
        public string priorityTitle { get; set; }
        public string recurrence { get; set; }
        public int? reviewStatusId { get; set; }
        public string reviewStatus { get; set; }
    }
}
