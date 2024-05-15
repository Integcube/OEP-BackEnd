using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class IATrackingEvidence
    {
        [Key]
        public int iatEvidenceId { get; set; }
        public string aitEvidenceTitle { get; set; }
        public decimal score { get; set; }
        public int isDeleted { get; set; }
    }
}
