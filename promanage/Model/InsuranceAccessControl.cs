using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceAccessControl
    {
        [Key]
        public int iacId { get; set; }
        public int userId { get; set; }
        public int siteId { get; set; }
        public int isPoc { get; set; }
    }
}
