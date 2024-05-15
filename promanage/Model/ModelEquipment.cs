using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ModelEquipment
    {
        [Key]
        public int fleetEquipId { get; set; }
        public string title { get; set; }
        public int equipmentTypeId { get; set; }
        public int oemId { get; set; }
        public int isDeleted { get; set; }
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set;}
    }
}
