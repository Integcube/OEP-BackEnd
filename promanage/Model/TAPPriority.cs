using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAPPriority
    {
        [Key]
        public int priorityId { get; set; }
        public string priorityTitle { get; set; }
        public decimal score { get; set; }
        public int isDeleted { get; set; }
    }
}
