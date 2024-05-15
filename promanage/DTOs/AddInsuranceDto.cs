using Microsoft.AspNetCore.Http;
using System;

namespace ActionTrakingSystem.DTOs
{
    public class uploadIRFilesDto
    {
        public IFormFile recommendationReport { get; set; }
        public string irId { get; set; }
        public string userId { get; set; }
    }
    public class AddInsuranceDto
    {
        public InsuranceRecommendationDto recommendation { get; set; }
        public int userId { get; set; }
    }
    public class IRApiData
    {
        public IRFilter filter { get; set; }
        public int userId { get; set; }
    }
    public class IRApiDataList
    {
        public int userId { get; set; }
        public string regionList { get; set; }
        public string clusterList { get; set; }
        public string siteList { get; set; }
        public string sourceList { get; set; }
        public string priorityList { get; set; }
        public string nomacStatus { get; set; }
        public string insuranceStatus { get; set; }
        public string yearList { get; set; }
        public string proactiveList { get; set; }
    }
    public class IRFilter
    {

        public DateTime? startData { get; set; } = null;
        public DateTime? endDate { get; set; } = null;
        public int regionId { get; set; } = -1;
        public int siteId { get; set; } = -1;
        public int sourceId { get; set; } = -1;
        public int nomacStatusId { get; set; } = -1;
        public int insuranceStatusId { get; set; } = -1;
    }
    public class InsuranceRecommendationDto 
    {
        public int recommendationId { get; set; }
        public string title { get; set; }
        public string insuranceRecommendation { get; set; }
        public string referenceNumber { get; set; }
        public int? priorityId { get; set; }
        public string priorityTitle { get; set; }
        public int? insurenceStatusId { get; set; }
        public string insurenceStatusTitle { get; set; }
        public int? nomacStatusId { get; set; }
        public string nomacStatusTitle { get; set; }
        public string latestStatus { get; set; }
        public DateTime? targetDate { get; set; }
        public string siteUpdates { get; set; }
        public string pcComments { get; set; }
        public string type { get; set; }
        public string expectedBudget { get; set; }
        public string siteTitle { get; set; }
        public int? siteId { get; set; }
        public string significance { get; set; }
        public string proactiveReference { get; set; }
        public int? proactiveId { get; set; }
        public string proactiveTitle { get; set; }
        public string regionTitle { get; set; }
        public int? regionId { get; set; }
        public int? sourceId { get; set; }
        public string sourceTitle { get; set; }
        public int? documentTypeId { get; set; }
        public string documentTypeTitle { get; set; }
        public string year { get; set; }
        public int? recommendationTypeId { get; set; }
        public string recommendationTypeTitle { get; set; }
        public string report { get; set; }
      
    }

}
