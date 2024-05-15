using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class IATrackingStatus
    {
        [Key]
        public int statusId { get; set; }
        public string statusTitle { get; set; }
        public int isDeleted { get; set; }
        public decimal score { get; set; }
    }
}
