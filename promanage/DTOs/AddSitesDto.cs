using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ActionTrakingSystem.DTOs
{
    public class siteTechnologyDto {
        public string technologyTitle { get; set; }
        public int technologyId { get; set; }
    }
    public class filterSiteDto 
    {
        public int userId { get; set; }
        public string regionList { get; set; } 
        public string countryList { get; set; } 
        public string technologyList { get; set; }
        public string clusterList { get; set; }

    }

    public class sSiteDto
    {
        public saveSiteDto site { get; set; }
        public int userId { get; set; }
    }
    public class saveSiteDto
    {
        public SitesI sites { get; set; }
        public siteTechnologyDto[]? techlogies { get; set; }
    }
    public class AddSitesDto
    {
        public SitesI site { get; set; }
        public int userId { get; set; }
    }
    public class FileDto
    {
        public IFormFile fileList { get; set; }
    }
    public class uploadFilesDto
    {
        public IFormFile? insuranceReport { get; set; }
        public IFormFile? tilsReport { get; set; }
        public string siteId { get; set; }
        public string userId { get; set; }

    }
    public class SitesI
    {
        public int siteId { get; set; }
        public string siteName { get; set; }
        //public IFormFile? insurenceReport { get; set; }
        //public IFormFile? tilsReport { get; set; }
        public string country { get; set; }
        public int? countryId { get; set; }
        public int? clusterId { get; set; }
        public string region { get; set; }
        public int? regionId { get; set; }
        public string s4hanaCode { get; set; }
        public string onmName { get; set; }
        public string projectName { get; set; }
        public string projectCompany { get; set; }
        public string siteDescription { get; set; }
        public string projectStatus { get; set; }
        public int? projectStatusId { get; set; }
        public DateTime? projectCOD { get; set; }
        public DateTime? onmContractExpiry { get; set; }
        public DateTime? insuranceLastReportDate { get; set; }
        public DateTime? insuranceNextAuditDate { get; set; }
        public int? sitePGMId { get; set; }
        public string sitePGMName { get; set; }
        public int? insurancePOCId { get; set; }
        public string insurancePOC { get; set; }
        public string insuranceSummary { get; set; }
        public string tilsSummary { get; set; }
        public string siteEMO { get; set; }
        public int? siteEMOId { get; set; }
        public int? tilPOCId { get; set; }
        public string tilPOC { get; set; }
    }
}
