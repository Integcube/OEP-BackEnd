using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class SiteEquipment
    {
        [Key]
        public int equipmentId { get; set; }
        public int? siteId { get; set; }
        public int? fleetEquipmentId { get; set; }
        public string unitSN { get; set; }
        //public DateTime? nextOutage { get; set; }
        //public int? outageTypeId { get; set; }
        public string details { get; set; }
        public string siteUnit { get; set; }
        public int? responsible { get; set; }
        public DateTime? unitCOD { get; set; }
        public string unit { get; set; }
        public int? isDeleted { get; set; }
        public DateTime createdDate { get; set; }
        public int? createdBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public int? modifiedBy { get; set; }
        public int? isGroup { get; set; }
        public string groupedEquipments { get; set; }

    }
}
