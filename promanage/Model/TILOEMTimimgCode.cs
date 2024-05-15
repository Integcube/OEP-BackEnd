using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILOEMTimimgCode
    {
        [Key]
        public int timingId { get; set; }
        public string timingCode { get; set; }

        public int isDeleted { get; set; }

    }
}
