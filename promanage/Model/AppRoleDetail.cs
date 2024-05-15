using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class AppRoleDetail
    {
        [Key]
        public int ruId { get; set; }
        public int roleId { get; set; }
        public int isDeleted { get; set; }
        public int menuId { get; set; }

    }
}
