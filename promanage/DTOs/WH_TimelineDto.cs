using System.Collections.Generic;
using System;

namespace ActionTrakingSystem.DTOs
{
    public class WH_TimelineDto
    {
        public string siteName { get; set; }
        public string unit { get; set; }
        public int year { get; set; }
        public decimal runningHours { get; set; }
        public string outageTitle { get; set; }
    }
    public class WH_SiteDto
    {
        public int siteId { get; set; }
    }
    public class FlattenedSite
    {
        public int siteId { get; set; }
        public string siteName { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public DateTime startDate { get; set; }
        public decimal startHours { get; set; }
        public DateTime onmContractExpiry { get; set; }
        public int yearId { get; set; }
        public decimal yearlyTotal { get; set; }
        public string outageTitle { get; set; }
        public int? outageId { get; set; }
        //public DateTime? OutageStartDate { get; set; }
        //public DateTime? OutageEndDate { get; set; }
    }

    public class FlattenedData
    {
        public List<FlattenedSite> Sites { get; set; } = new List<FlattenedSite>();
    }
}
