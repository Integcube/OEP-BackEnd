using System;

namespace ActionTrakingSystem.DTOs
{
    public class ChemistryActionPlanDtos
    {
    }
    public class CAPObservationUserDto
    {
        public CAPObservationDto observation { get; set; }
        public int userId { get; set; }
    }
    public class CAPGetObservation
    {
        public int userId { get; set; }
        public string regionList { get; set; }
        public string siteList { get; set; }
    }
    public class CAPActionUserDto
    {
        public CAPActionDto action { get; set; }
        public int userId { get; set; }
    }
    public class CAPRegionUserDto
    {
        public int regionId { get; set; }
        public int userId { get; set; }
    }
    public class CAPActionFilterUserDto
    {
       public string regionList { get; set; }
        public string siteList { get; set; }
        public string statusList { get; set; }

        public int userId { get; set; }
    }
    public class CAPActionFilterObj
    {
        public int regionId { get; set; }
        public int siteId { get; set; }
        public int priorityId { get; set; }
        public int statusId { get; set; }
    }
    public class CAPActionDto
    {
        public int? observationId { get; set; }
        public string observationTitle { get; set; }
        public int? actionId { get; set; }
        public string actionTitle { get; set; }
        public int? priorityId { get; set; }
        public string priorityTitle { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? targetDate { get; set; }
        public string remarks { get; set; }
        public int? deptId { get; set; }
        public string deptTitle { get; set; }
        public int? userId { get; set; }
        public string userName { get; set; }
        public int? statusId { get; set; }
        public string statusTitle { get; set; }
        public string referenceNumber { get; set; }
        public string suggestion { get; set; }
    }
    public class CAPIdsDto
    {
        public int observationId { get; set; }
        public int userId { get; set; }
        public int actionId { get; set; }
    }
    public class CAPObservationDto
    {
        public int observationId { get; set; }
        public string observationTitle { get; set; }
        public int? plantId { get; set; }
        public string plantTitle { get; set; }
        public int? regionId { get; set; }
        public string regionTitle { get; set; }
        public int? actionId { get; set; }
        public string actionTitle { get; set; }
        public int? priorityId { get; set; }
        public string priorityTitle { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? targetDate { get; set; }
        public string remarks { get; set; }
        public int? deptId { get; set; }
        public string deptTitle { get; set; }
        public int? userId { get; set; }
        public string userName { get; set; }
        public int? statusId { get; set; }
        public string statusTitle { get; set; }
        public string referenceNumber { get; set; }
        public string suggestion { get; set; }
    }
}
