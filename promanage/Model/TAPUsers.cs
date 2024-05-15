using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAPUsers
    {
        [Key]
        public int tapUserId { get; set; }
        public int userId { get; set; }
        public int tapId { get; set; }
        public int isDeleted { get; set; }
    }
}
