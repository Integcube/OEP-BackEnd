using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
    [Table("Project_StartPhases")]
    public class ProjectStartPhase
    {
        [Key]
        public int StartPhaseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PhaseId { get; set; }
        public int PlanId { get; set; }
        public int PgmId { get; set; }
        public int startplanId { get; set; }
        

    }
}
