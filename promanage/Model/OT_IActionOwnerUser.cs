using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_IActionOwnerUser
    {
        [Key]
        public int actionOwnerUserId { get; set; }
        public int actionOwnerId { get; set; }
        public int userId { get; set; }
    }
}
