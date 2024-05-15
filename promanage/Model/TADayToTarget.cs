using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TADayToTarget
    {
        [Key]
        public int dayId { get; set; }
        public string title { get; set; }
        public int isDeleted { get; set; }
        public decimal? score { get; set; }
    }
}
