using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILAccessControl
    {
        [Key]
        public int tacId { get; set; }
        public int userId { get; set; }
        public int siteId { get; set; }
        public int isPoc { get; set; }
    }
}
