using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveAudit
    {
        [Key]
        public int auditId { get; set; }
        public string auditTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
