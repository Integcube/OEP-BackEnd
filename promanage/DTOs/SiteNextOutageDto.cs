using System;

namespace ActionTrakingSystem.DTOs
{
    public class SiteNextOutageDto
    {
        public int snoId { get; set; }
        public int siteId { get; set; }
        public string siteTitle { get; set; }
        public int outageTypeId { get; set; }
        public string outageTitle { get; set; }
        public int outageLevel { get; set; }
        public DateTime nextOutageDate { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
    }
    public class SiteNextOutageUser
    {
        public int userId { get; set; }
        public SiteNextOutageDto siteNextOutage { get; set; }
    }


}
