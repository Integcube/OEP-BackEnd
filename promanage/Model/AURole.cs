using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class AURole
    {
        [Key]
        public int auId { get; set; }
        public int userId { get; set; }
        public int roleId { get; set; }
        public int isDeleted { get; set; }

    }
}
