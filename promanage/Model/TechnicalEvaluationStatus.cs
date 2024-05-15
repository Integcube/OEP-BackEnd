using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TechnicalEvaluationStatus
    {
        [Key]
        public int tesId { get; set; }
        public string title { get; set; }
        public int isDeleted { get; set; }
    }
}
