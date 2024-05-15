using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_PhaseReadinessDescriptionAO
    {
        [Key]
        public int prdaoId { get; set; }
        public int actionOwnerId { get; set; }
        public int phaseReadId { get; set; }
        public int isDeleted { get; set; }
    }
}
