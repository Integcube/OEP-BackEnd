using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveProjectPhase
    {
        [Key]
        public int projectPhaseId { get; set; }
        public string projectPhaseTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
