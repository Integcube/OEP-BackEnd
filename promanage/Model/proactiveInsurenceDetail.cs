using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class proactiveInsurenceDetail
    {
        [Key]
        public int pidId { get; set; }
        public int irId { get; set; }
        public int proactiveId { get; set; }
        public int isDeleted { get; set; }
    }
}
