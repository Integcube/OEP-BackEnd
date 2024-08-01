using System;
using System.Collections.Generic;

namespace ActionTrakingSystem.DTOs
{
    public class ManpowerPlanDto
    {
        public List<ManpowerPlan> ManpowerPlans { get; set; }
        public int userId { get; set; }
    }


    public class ManpowerPlan
    {
        public int PositionId { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public int? ApprovedManpower { get; set; }
        public int? numberOfPositionActual { get; set; }
        public int? NumberOfPosition { get; set; }
        public int PositionType { get; set; }
        public int SiteId { get; set; }
        public string PeopleName { get; set; }
        
    }



}
