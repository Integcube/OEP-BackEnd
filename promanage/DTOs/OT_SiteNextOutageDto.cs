using System;

namespace ActionTrakingSystem.DTOs
{
    public class OT_SiteNextOutageDto
    {
        public int snoId { get; set; }
        //public int siteId { get; set; }
        //public string siteTitle { get; set; }
        public int equipmentId { get; set; }
        public string unit { get; set; }
        public int outageId { get; set; }
        public string outageTitle { get; set; }
        public string nextOutageDate { get; set; }
        public decimal runningHours { get; set; }
        public decimal wceHours { get; set; }
        public decimal? outageDurationInDays { get; set; }
        public DateTime? actualStartDate { get; set; }
        public DateTime? actualEndDate { get; set; }
    }
    public class OT_GetNextOutageDto
    {
        public int userId { get; set; }
        public OT_GetNextOutageFilter filter { get; set; }

    }
    public class OT_GetNextOutageFilter
    {
        public int userId { get; set; }
        public int regionId { get; set; }
        public int clusterId { get; set; }
        public int siteId { get; set; }
        public int equipmentId { get; set; }
        public int outageId { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

    }
    public class OT_SiteNextOutageUser
        {
            public int userId { get; set; }
            public OT_SiteNextOutageDto siteNextOutage { get; set; }
        }

    public class OT_GetEquipments
    {
        public int siteId { get; set; }
    }
}
