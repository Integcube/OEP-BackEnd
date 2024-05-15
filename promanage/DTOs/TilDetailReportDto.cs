namespace ActionTrakingSystem.DTOs
{
    public class TilDetailReportDto
    {
        public string siteName { get; set; }
        public int? siteId { get; set; }
        public string unit { get; set; }
        public int? actions { get; set; }
        public int? closed { get; set; }
        public int? opened { get; set; }
        public int? inProgress { get; set; }
        public int? regionId { get; set; }
        public int? clusterId { get; set; }
        public int? timingId { get; set; }
        public int? focusId { get; set; }
        public int? severityId { get; set; }
        public int? unitId { get; set; }
        public int? statusId { get; set; }
        public string oemSeverityTitle { get; set; }
        public string timingCode { get; set; }
        public string focusTitle { get; set; }
    }
}
