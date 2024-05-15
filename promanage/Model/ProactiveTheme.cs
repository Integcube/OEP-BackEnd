using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveTheme
    {
        [Key]
        public int themeId { get; set; }
        public string themeTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
