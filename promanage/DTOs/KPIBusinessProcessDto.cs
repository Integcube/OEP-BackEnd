using System;

namespace ActionTrakingSystem.DTOs
{
    public class getKPIBusinessDto 
    {
        public int userId { get; set; }
        public int siteId { get; set; }
        public int monthId { get; set; }
        public int yearId { get; set; }
    }

    public class KPIBusinessUserDto
    {
        public int userId { get; set; }
        public int siteId { get; set; }
        public int monthId { get; set; }
        public int yearId { get; set; }
        public KPIBusinessProcessDto[] kpi { get; set; }
    }
    public class KPIBusinessProcessDto
    {
        public int groupId { get; set; }
        public string groupTitle { get; set; }
        public string groupCode { get; set; }
        public int indicatorId { get; set; }
        public int infoId { get; set; }
        public string indicatorCode { get; set; }
        public string indicatorTitle { get; set; }
        public int weightageId { get; set; }
        public int siteId { get; set; }
        public string siteName { get; set; }
        public Boolean? notApplicable { get; set; }
        public decimal? value { get; set; }
        public string annual { get; set; }
        public string comment { get; set; }
        public int addedDate { get; set; }
    }
}
