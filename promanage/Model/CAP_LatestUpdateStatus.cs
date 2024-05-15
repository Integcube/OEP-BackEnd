using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_LatestUpdateStatus
    {
        [Key]
        public int lusId { get; set; }
        public string status { get; set; }
        public decimal? score { get; set; }
        public int isDeleted { get; set; }
    }
}
