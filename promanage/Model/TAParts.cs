using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAParts
    {
        [Key]
        public int partId { get; set; }
        public string partTitle { get; set; }
        public int isDeleted { get; set; }
        public decimal? score { get; set; }
    }
}
