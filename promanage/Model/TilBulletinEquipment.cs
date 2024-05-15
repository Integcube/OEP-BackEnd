using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TilBulletinEquipment
    {
        [Key]
        public int tilEquipmentId { get; set; }
        public string title { get; set; }
        public int isDeleted { get; set; }
    }
}
