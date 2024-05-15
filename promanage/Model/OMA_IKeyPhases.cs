using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_IKeyPhases
    {
        [Key]
        public int keyPhaseId { get; set; }
        public string keyPhaseTitle { get; set; }
        public string keyPhaseCode { get; set; }
        public int isDeleted { get; set; }
    }
}
