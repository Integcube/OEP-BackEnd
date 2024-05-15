using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class SitesTechnology
    {
        [Key]
        public int stId { get; set; }
        public int techId { get; set; }
        public int siteId { get; set; }
        public int isDeleted { get; set; }
    }
}
