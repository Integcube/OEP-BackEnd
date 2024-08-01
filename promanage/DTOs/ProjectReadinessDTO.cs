using Microsoft.AspNetCore.Http;
using System;

namespace ActionTrakingSystem.DTOs
{
    public class ProjectReadinessDTO
    {
       public string PlanTitle { get; set; }
        public string? phasetitle { get; set; }
        public int? planId { get; set; }
        public decimal? Phaseweightage { get; set; }
        public int? taskId { get; set; }
        public string? tasktitle { get; set; }
        public decimal? idealScore { get; set; }
        public decimal? actualScore { get; set; }
        public int responsibility { get; set; }
        public DateTime? targetdate { get; set; }
        public string? Remarks { get; set; }
        public int displayOrder { get; set; }
        public int phaseId { get; set; }
        public string? Username { get; set; }
        public decimal? ElementScore { get; set; }
        public decimal? TotalIdealScore { get; set; }
        public decimal? TotalActualScore { get; set; }

    }



    public class ProjectReadinessphase
    {
        public string title { get; set; }
        public decimal weightage { get; set; }
        public int planId { get; set; }
        public int phaseId { get; set; }
        public int createdBy { get; set; }
        public int displayOrder { get; set; }

    }


    public class DocInfoReadiness
    {

        public string Date { get; set; }
        public int SiteId { get; set; }
        public int type { get; set; }
        public int taskId { get; set; }

    }

    public class UploadFileDtoReadiness
    {
        public IFormFile? report { get; set; }
        public string siteId { get; set; }
        public string date { get; set; }
        public string remarks { get; set; }
        public string fileName { get; set; }
        public string userId { get; set; }
        public string type { get; set; }
        public string taskId { get; set; }
    }
}
