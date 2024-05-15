using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_SiteControl
    {
        [Key]
        public int siteControlId { get; set; }
        public int siteId { get; set; }
        public int technologyId { get; set; }
        public int programId { get; set; }
    }
}
