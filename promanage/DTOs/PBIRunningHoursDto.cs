using System;
using System.Collections.Generic;

namespace ActionTrakingSystem.DTOs
{
    //public class timeLine
    //{
    //    public List<PBIRunningHoursSitesDto> timeLine { get; set; }
    //}
    public class PBIRunningHoursSitesDto
    {
        public int siteId { get; set; }
        public string siteName { get; set; }
        public int regionId { get; set; }
        public string regionTitle { get; set; }
        public string eqType { get; set; }
        public int clusterId { get; set; }
        public string clusterTitle { get; set; }
        public List<PBIRunningHoursEqpDto> eqps { get; set; }
    }
    public class PBIRunningHoursEqpDto
    {
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public DateTime startDate { get; set; }
        public decimal startHours { get; set; }
        public decimal wceStartHours { get; set; }
        public DateTime onmContractExpiry { get; set; }
        public List<PBIRunningHoursYearlyDto> yearly { get; set; }
    }
    public class PBIRunningHoursYearlyDto
    {
        public int yearId { get; set; }
        public decimal yearlyTotal { get; set; }
        public List<PBIRunningHoursOuatgesDto> outages { get; set; }


    }
    public class PBIRunningHoursOuatgesDto
    {
        public string outageTitle { get; set; }
        public int outageId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }

    public class imutableProps
    {
        public string outageTitle { get; set; }
        public string eqType { get; set; }
        public int outageId { get; set; }
        public decimal outageWCEHours { get; set; }
        public decimal outageWOHHours { get; set; }
        public DateTime nextOutageDate { get; set; }
        public string colorCode { get; set; }
        public decimal counter { get; set; }
        public decimal counter2 { get; set; }
        public int equipmentId { get; set; }
        public string validate { get; set; }
        public decimal outageDurationInDays { get; set; }

    }
}
