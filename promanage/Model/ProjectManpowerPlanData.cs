using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("Project_ManpowerPlanData")]
    public class ProjectManpowerPlanData
    {
        [Key]
        public int id { get; set; }
        public int position { get; set; }
        public int positionType { get; set; }
        public DateTime month { get; set; }
        public int? approvedManpower { get; set; }
        public int? numberOfPosition { get; set; }
        public int createdBy { get; set; }
        public DateTime? createdOn { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int siteId { get; set; }
        public string title { get; set; }
        public string peopleName { get; set; }
        public int? numberOfPositionActual { get; set; }
        


    }
}
