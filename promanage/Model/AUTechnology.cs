using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class AUTechnology
    {
        [Key]
        public int autId { get; set; }
        public int userId { get; set; }
        public int technologyId { get; set; }
        public int isDeleted { get; set; }

    }
}
