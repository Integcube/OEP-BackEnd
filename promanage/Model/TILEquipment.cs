using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILEquipment
    {
        [Key]
        public int equipmentId { get; set; }
        public string equipmentTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
