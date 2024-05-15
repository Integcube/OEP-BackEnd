using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Mail;

namespace ActionTrakingSystem.DTOs
{
    public class InsuranceRecommendatioSpDto
    {

        public int? recommendationId { get; set; }
        public string? title { get; set; }
        public string? insuranceRecommendation { get; set; }
        public string? referenceNumber { get; set; }
        public int? priorityId { get; set; }
        public string? priorityTitle { get; set; }
        public int? insurenceStatusId { get; set; }
        public string? insurenceStatusTitle { get; set; } 
        public int? nomacStatusId { get; set; } 
        public string? nomacStatusTitle { get; set; } 
        public string? latestStatus { get; set; } 
        public DateTime? targetDate { get; set; }
        public string? siteUpdates { get; set; } 
        public string? pcComments { get; set; } 
        public string? type { get; set; } 
        public string? expectedBudget { get; set; } 
        public string? siteTitle { get; set; } 
        public int? siteId { get; set; } 
        public string? significance { get; set; } 
        public string? proactiveReference { get; set; }
        public int? proactiveId { get; set; } 
        public string? proactivetitle { get; set; }
        public string? regionTitle { get; set; } 
        public int? regionId { get; set; }
        public int? sourceId { get; set; } 
        public string? sourceTitle { get; set; } 
        public int? documentTypeId { get; set; } 
        public string? documentTypeTitle { get; set; } 
        public string? year { get; set; } 
        public int? recommendationTypeId { get; set; } 
        public string? recommendationTypeTitle { get; set; } 
        public string? report { get; set; }

        public string? reportAttached { get; set; }
        public string? reportName { get; set; } 
        public int? clusterId { get; set; }
        public string? clusterTitle { get; set; } 

    }
}
