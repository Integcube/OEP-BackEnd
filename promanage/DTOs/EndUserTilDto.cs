using System;

namespace ActionTrakingSystem.DTOs
{
    public class EndUserTilDto
    {
        public int acId { get; set; }
        public string tilNumber { get; set; }
        public int? tilId { get; set; }
        public string action { get; set; }
        public string actionDescription { get; set; }
        public string assignedto { get; set; }
        public int? assignedId { get; set; }
        public string actionCategoryTitle { get; set; }
        public string outageTitle { get; set; }
        public string siteStatusDetail { get; set; }
        public int? budgetId { get; set; }
        public string budgetTitle { get; set; }
        public int? planningId { get; set; }
        public string planningTitle { get; set; }
        public string finalImplementationTitle { get; set; }
        public int? finalImplementationId { get; set; }
    }
    public class EUFilterObj
    {
        public EndUserFilter filter { get; set; }
        public int userId { get; set; }
    }
    public class EUFilterList
    {
        public int userId { get; set; }
        public string regionList { get; set; }
        public string finalImpList { get; set; }
        public string siteList { get; set; }
        public string focusList { get; set; }
        public string severityList { get; set; }
        public string clusterList { get; set; }
        public string equipmentList { get; set; }
        public string sapList { get; set; }
        public string statusList { get; set; }
        public string priorityList { get; set; }
        public string unitStatusList { get; set; }
        public string daysList { get; set; }
        public string quarterList { get; set; }
        public string yearList { get; set; }
        public string equipmentTypeList { get; set; }

    }
    public class EndUserFilter
    {
        public DateTime? startDate { get; set; } = null;
        public DateTime? endDate { get; set; } = null;
        public int sap { get; set; } = -1;
        public int status { get; set; } = -1;
        public int equipment { get; set; } = -1;
        public string daysToTarget { get; set; } = "";
        public int regionId { get; set; } = -1;
        public int siteId { get; set; } = -1;
    }
}
