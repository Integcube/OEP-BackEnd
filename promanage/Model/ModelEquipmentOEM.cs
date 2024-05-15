using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ModelEquipmentOEM
    {
        [Key]
        public int oemId { get; set; }
        public string oemTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
