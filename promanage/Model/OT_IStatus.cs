using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_IStatus
    {
        [Key]
        public int statusId { get; set; }
        public string statusTitle { get; set; }
    }
}
