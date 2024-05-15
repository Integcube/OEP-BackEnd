using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class IATrackingDaysStatus
    {
        [Key]
        public int iatDayStatusId { get; set; }
        public string iatDayStatus { get; set; }
        public decimal score { get; set; }
        public int isDeleted { get; set; }
    }
}
