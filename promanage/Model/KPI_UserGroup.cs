using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class KPI_UserGroup
    {
        [Key]
        public int userGroupId { get; set; }
        public int userId { get; set; }
        public int groupId { get; set; }
        public int isDeleted { get; set; }
    }
}
