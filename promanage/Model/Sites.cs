using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class Sites
    {
        [Key]
        public int siteId { get; set; }
        public string siteName { get; set; }
        public int? regionId { get; set; }
        public int? countryId { get; set; }
        public string projectCompany { get; set; }
        public string siteDescription { get; set; }
        public int? projectStatusId { get; set; }
        public DateTime? projectCOD { get; set; }
        public DateTime onmContractExpiry { get; set; }
        public DateTime? insuranceLastReportDate { get; set; }
        public DateTime? insuranceNextAuditDate { get; set; }
        public int? sitePGMId { get; set; }
        public int? siteEMOId { get; set; }

        public int? insurancePOCId { get; set; }
        public int? tilPocId { get; set; }
        public string insuranceSummary { get; set; }
        public string tilsSummary { get; set; }
        public int? clusterId { get; set; }
        public int? region2 { get; set; }
        public int? otValid { get; set; }
        public string s4hanaCode { get; set; }
        public string projectName { get; set; }
        public string onmName { get; set; }
        public DateTime? createdDate { get; set; }
        public int? createdBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }

    }
}
