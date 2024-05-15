using Microsoft.AspNetCore.Http;
using System;

namespace ActionTrakingSystem.DTOs
{
    public class AssignInsurenceTrackerDto
    {
        public AssignInsurenceDto insurenceAction { get; set; }
        public int userId { get; set; }
    }
    public class AssignInsurenceStatusDto
    {
        public AssignInsurenceDto insurenceAction { get; set; }
        public int userId { get; set; }
    }
    public class AssignInsurenceFilter
    {
        public DateTime? startData { get; set; }
        public DateTime? endDate { get; set; }
        public int regionId { get; set; }
        public int siteId { get; set; }
        public int department { get; set; }
        public int source { get; set; }
    }
    public class getIRDto
    {
        public int userId { get; set; }
        public int recommendationId { get; set; }
    }
    public class AssignInsurenceUserFilter
    {
        //public AssignInsurenceFilter filter { get; set; }
        public int userId { get; set; }
        public string regionList { get; set; }
        public string siteList { get; set; }
        public string sourceList { get; set; }
        public string clusterList { get; set; }
        public string department { get; set; }
        public string priorityList { get; set; }

    }
    public class InsuranceTrackerFilter
    {
        public DateTime? startDate { get; set; } = null;
        public DateTime? endDate { get; set; } = null;
        public int sourceId { get; set; }
        public int departmentId { get; set; } = -1;
        public int statusId { get; set; } = -1;
        public string daysToTarget { get; set; }
        public int regionId { get; set; } = -1;
        public int siteId { get; set; } = -1;

    }
    public class DaysToTargetDto
    {
        public int iatId { get; set; }
        public DateTime targetDate { get; set; }
        public int statusId { get; set; }
        public string dayToTarget { get; set; }
    }
    public class InsuranceTrackerUserFilter
    {
        public InsuranceTrackerFilter filter { get; set; }
        public int userId { get; set; }
        public int pocId { get; set; } = -1;

    }
    public class InsuranceTrackerUserFilterList
    {
        public int userId { get; set; }
        public string regionList { get; set; }
        public string siteList { get; set; }
        public string sourceList { get; set; }
        public string quaterList { get; set; }
        public string statusList { get; set; }
        public string dayList { get; set; }
        public string companyList { get; set; }
        public string priorityList { get; set; }
        public string clusterList { get; set; }
        public string yearList { get; set; }
        public string issueYearList { get; set; }
    }
    public class iatFileDto
    {
        public IFormFile? iatReport { get; set; }
        public string iatId { get; set; }
        public string userId { get; set; }
        


    }
    public class iatFileListDto
    {
        public int iatId { get; set; }
        public int userId { get; set; }



    }
    public class AssignInsurenceDtoR
    {
        public int insurenceActionTrackerId { get; set; }
        public int? recommendationId { get; set; }
        public string recommendationTitle { get; set; }
        public string recommendationReference { get; set; }
        public string action { get; set; }
        public DateTime? targetDate { get; set; }
        public int? statusId { get; set; }
        public string statusTitle { get; set; }
        public decimal? statusScore { get; set; }
        public int? companyId { get; set; }
        public string companyTitle { get; set; }
        public string comments { get; set; }
        public DateTime? closureDate { get; set; }
        public int? evidenceAvailableId { get; set; }
        public string evidenceAvailable { get; set; }
        public decimal? evidenceAvailableScore { get; set; }
        public int? dayStatusId { get; set; }
        public string dayStatusTitle { get; set; }
        public decimal? dayStatusScore { get; set; }
        public decimal? calcStatus { get; set; }
        public decimal? calcEvid { get; set; }
        public decimal? calcDate { get; set; }
        public string completionScore { get; set; }
        public string daysToTarget { get; set; }
        public string scoreDetails { get; set; }
        public int? siteId { get; set; }
        public string siteTitle { get; set; }
        public int? regionId { get; set; }
        public string regionTitle { get; set; }
        public int? sourceId { get; set; }
        public string sourceTitle { get; set; }
        public bool? reportAttahced { get; set; }
        public string reportName { get; set; }
    }
    public class AssignInsurenceDto
    {
 
        public int? assignedToId { get; set; }
        public string assignedToTitle { get; set; }
        public int insurenceActionTrackerId { get; set; }
        public int? recommendationId { get; set; }
        public string recommendationTitle { get; set; }
        public string recommendationReference { get; set; }
        public string action { get; set; }
        public DateTime? targetDate { get; set; }
        public int? statusId { get; set; }
        public string statusTitle { get; set; }
        public decimal? statusScore { get; set; }
        public int? companyId { get; set; }
        public string companyTitle { get; set; }
        public string comments { get; set; }
        public DateTime? closureDate { get; set; }
        public int? evidenceAvailableId { get; set; }
        public string evidenceAvailable { get; set; }
        public decimal? evidenceAvailableScore { get; set; }
        public int? dayStatusId { get; set; }
        public string dayStatusTitle { get; set; }
        public decimal? dayStatusScore { get; set; }
        public decimal? calcStatus { get; set; }
        public decimal? calcEvid { get; set; }
        public decimal? calcDate { get; set; }
        public string completionScore { get; set; }
        public string daysToTarget { get; set; }
        public string scoreDetails { get; set; }
        public int? siteId { get; set; }
        public string siteTitle { get; set; }
        public int? regionId { get; set; }
        public string regionTitle { get; set; }
        public int? sourceId { get; set; }
        public string sourceTitle { get; set; }
        public string adminComment { get; set; }
        public string reviewerComment { get; set; }
        public bool? isCompleted { get; set; }
        public bool? rework { get; set; }
        public bool? clusterReviewed { get; set; }
    }
}
