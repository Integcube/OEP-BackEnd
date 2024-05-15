using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILOEMSeverity
    {
        [Key]
        public int oemSeverityId { get; set; }
        public string oemSeverityTitle { get; set; }

        public int isDeleted { get; set; }

    }
}
