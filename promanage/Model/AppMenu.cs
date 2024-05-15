using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class AppMenu
    {
        [Key]
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public int ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public decimal Status { get; set; }
        public string Role { get; set; }
        public string IconType { get; set; }
        public string Class { get; set; }
        public string Badge { get; set; }
        public string BadgeClass { get; set; }
        public bool GroupTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
