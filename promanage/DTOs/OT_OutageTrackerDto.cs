using Microsoft.AspNetCore.Http;
using System;

namespace ActionTrakingSystem.DTOs
{
    public class OutageTrackerModel
    {
        public decimal? progress { get; set; }
        public int siteId { get; set; }
        public string siteTitle { get; set; }
        public int phaseId { get; set; }
        public int phaseNumber { get; set; }
        public string phaseTitle { get; set; }
        public string name { get; set; }
        public int? clusterId { get; set; }
        public string clusterTitle { get; set; }
        public int phaseReadId { get; set; }
        public string phaseReadDesc { get; set; }
        //public string remarks { get; set; }
        //public string filePath { get; set; }
        //public string fileName { get; set; }
        //public decimal progress { get; set; }
        public bool notApplicable { get; set; }
        public int statusId { get; set; }
        public string statusTitle { get; set; }
        public int potId { get; set; }
        public int snoId { get; set; }
        public int outageId { get; set; }
        public DateTime nextOutageDate { get; set; }
        public string outageTitle { get; set; }
        public int phaseDurId { get; set; }
        public decimal durationMonths { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
    public class OT_OutageApiDataDto
    {

        public OT_OutageTrackerDto outageTracker { get; set; }
        public monthlyOTDto monthlyData { get; set; }

    }
    public class monthlyOTDto
    {
        public int yearId { get; set; }
        public int monthId { get; set; }
        public string remarks { get; set; }
        public decimal? progress { get; set; }
        public int progressId { get; set; }

    }
    public class OT_OutageTrackerFileDto
    {
        public int userId { get; set; }
        public int fileId { get; set; }
    }
    public class OT_OutageTrackerUserDto
    {
        public int userId { get; set; }
        public OT_OutageApiDataDto action { get; set; }
    }
    public class OT_OutageTrackerUserSDto
    {
        public int userId { get; set; }
        public OT_OutageTrackerDto action { get; set; }
        
    }
    public class OT_OutageTrackerUserCDto
    {
        public int userId { get; set; }
        public string monthId { get; set; }
        public int potId { get; set; }
    }
    public class OT_DateCalc
    {
        public string monthId { get; set; }
        public DateTime date { get; set; }
    }
    public class OT_FilterUserDto
    {
        public int userId { get; set; }
        public OT_FilterDto filter { get; set; }
    }
    public class OT_FilterDto
    {
        public int phaseNumber { get; set; }
        public int siteId { get; set; }
        public int outageId { get; set; }
        public int status { get; set; }
        
    }
    public class OT_UploadFileDto
    {
        public IFormFile? report { get; set; }
        public string equipmentId { get; set; }
        public string phaseId { get; set; }
        public string phaseReadId { get; set; }
        public string outageDate { get; set; }
        public string remarks { get; set; }
        public string fileName { get; set; }
        public string userId { get; set; }


    }
    public class OT_OutageTrackerDto
    {
        public int siteId { get; set; }
        public string siteTitle { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public int phaseId { get; set; }
        public int phaseNumber { get; set; }
        public string phaseTitle { get; set; }
        public int phaseReadId { get; set; }
        public string phaseReadDesc { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public Boolean notApplicable { get; set; }
        public int potId { get; set; }
        public int snoId { get; set; }
        public int outageId { get; set; }
        public string outageTitle { get; set; }
        public DateTime nextOutageDate { get; set; }
    }
}
