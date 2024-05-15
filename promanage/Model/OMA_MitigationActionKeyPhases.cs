using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_MitigationActionKeyPhases
    {
        [Key]
        public int makpId { get; set; }
        public int actionId { get; set; }
        public int keyPhaseId { get; set; }
        public int isDeleted { get; set; }
    }
}
