using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_IPriority
    {
        [Key]
        public int priorityId { get; set; }
        public string priorityTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
