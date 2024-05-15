using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class AppRole
    {
        [Key]
        public int roleId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int isDeleted { get; set; }
    }
}
