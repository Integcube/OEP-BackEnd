using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_WSCObservation
    {
        [Key]
        public int wscId { get; set; }
        public int? plantId { get; set; }
        public string observation { get; set; }
        public DateTime? createdDate { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }

    }
}
