using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAPEquipment
    {
        [Key]
        public int tapEquipId { get; set; }
        public int eqId { get; set; }
        public int tapId { get; set; }
        public int isDeleted { get; set; }
    }
}
