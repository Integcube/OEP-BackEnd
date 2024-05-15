using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveExposure
    {
        [Key]
        public int exposureId { get; set; }
        public string exposureTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
