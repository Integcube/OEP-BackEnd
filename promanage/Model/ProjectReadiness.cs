using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionTrakingSystem.Model
{
    [Table("Project_Readiness")]
    public class ProjectReadiness
    {
        [Key]
        public int PlanId { get; set; }
        public string Title { get; set; }
        public int SiteId { get; set; }
        public int AssignTo { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public int IsDeleted { get; set; }
        
    }
}
