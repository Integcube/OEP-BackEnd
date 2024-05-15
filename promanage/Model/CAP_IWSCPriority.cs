using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_IWSCPriority
    {
        [Key]
        public int wscPriorityId { get; set; }
        public string priorityTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
