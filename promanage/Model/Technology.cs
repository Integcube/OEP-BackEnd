using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class Technology
    {
        [Key]
        public int techId { get; set; }
        public string name { get; set; }
        public string leader { get; set; }
        public string team { get; set; }
        public int isDeleted { get; set; }

    }
}
