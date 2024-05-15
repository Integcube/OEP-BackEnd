using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_IWSCDept
    {
        [Key]
        public int wscDeptId { get; set; }
        public string deptTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
