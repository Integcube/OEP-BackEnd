using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveCriticality
    {
        [Key]
        public int criticalityId { get; set; }
        public string criticalityTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
