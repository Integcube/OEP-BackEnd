using System.Collections.Generic;

namespace ActionTrakingSystem.DTOs
{
    public class TilActionReportDto
    {
        public int statusId { get; set; }
        public string statusTitle { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public int siteId { get; set; }
        public string siteName { get; set; }
        public int regionId { get; set; }
        public string regionTitle { get; set; }
        public int clusterId { get; set; }
        public string clusterTitle { get; set; }
        public string tilNumber { get; set; }
        public int unitId { get; set; }
        public int timingId { get; set; }
        public int focusId { get; set; }
        public int severityId { get; set; }
        public string oemSeverityTitle { get; set; }
        public string timingCode { get; set; }
        public string focusTitle { get; set; }
    }
    public class TilActionReportDtoList
    {
        public List<int> statusId { get; set; }
        public List<string> statusTitle { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public int siteId { get; set; }
        public string siteName { get; set; }
        public int regionId { get; set; }
        public string regionTitle { get; set; }
        public int clusterId { get; set; }
        public string clusterTitle { get; set; }
        public string tilNumber { get; set; }

    }
}
