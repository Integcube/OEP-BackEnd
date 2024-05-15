using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAStatus
    {
        [Key]
        public int tasId { get; set; }
        public string title { get; set; }
        public int isDeleted { get; set; }
        public decimal? score { get; set; }
    }
}
