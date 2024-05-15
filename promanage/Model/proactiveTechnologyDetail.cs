using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class proactiveTechnologyDetail
    {
        [Key]
        public int ptdId { get; set; }
        public int techId { get; set; }
        public int proactiveId { get; set; }
        public int isDeleted { get; set; }
    }
}
