using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAEvidence
    {
        [Key]
        public int evidenceId { get; set; }
        public string evidenceTitle { get; set; }
        public int isDeleted { get; set; }
        public decimal? score { get; set; } 
    }
}
