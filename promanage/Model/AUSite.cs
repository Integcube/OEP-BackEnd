using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class AUSite
    {
        [Key]
        public int ausId { get; set; }
        public int userId { get; set; }
        public int siteId { get; set; }
        public int isDeleted { get; set; }

    }
}
