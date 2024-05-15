using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveCategory
    {
        [Key]
        public int categoryId { get; set; }
        public string categoryTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
