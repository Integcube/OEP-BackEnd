using System;

namespace ActionTrakingSystem.DTOs
{
    public class FilterEqSiteDto
    {
        public int userId { get; set; }
        public int regionId { get; set; }
    }
    public class FilterEqUserDto 
    {
        public int userId { get; set; }
        public int siteId { get; set; }
    }
    public class getSiteEquiomentDto
    {

        public int userId { get; set; }
        public SiteEquipmentFilterDto filterObj { get; set; }
    }
    public class getSiteEquiomentFilterDto
    {

        public int userId { get; set; }
        public string regionList { get; set; }
        public string siteList { get; set; }
        public string clusterList { get; set; }
        public string modelList { get; set; }
        public string eqTypeList { get; set; }
        public string oemList { get; set; }
    }
    public class SiteEquipmentFilterDto
    {
        public int? oemId { get; set; } = -1;
        public int? equipmentTypeId { get; set; } = -1;
        public int? regionId { get; set; } = -1;
        public int? siteId { get; set; } = -1;
        public int? modelId { get; set; } = -1;
        public int? fleetEquipId { get; set; } = -1;
    }
    public class SiteEquipmentSaveDto
    {
        public int userId { get; set; }
        public SiteEquipmentDto? equipment { get; set; }
    }
    public class SiteEquipmentDto
    {
        public int equipmentId { get; set; }
        public int? regionId { get; set; }
        public string regionTitle { get; set; }
        public int? siteId { get; set; }
        public string siteTitle { get; set; }
        public string model { get; set; }
        public int? modelId { get; set; }
        public string unitSN { get; set; }
        public DateTime? nextOutage { get; set; }
        public string outageType { get; set; }
        public int? outageTypeId { get; set; }
        public string details { get; set; }
        public int? responsible { get; set; }
        public string responsibleName { get; set; }
        public DateTime? unitCOD { get; set; }
        public string unit { get; set; }
        
    }
}
