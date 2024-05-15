using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_LatestUpdate
    {
        [Key]
        public int updateId { get; set; }
        public int? siteId { get; set; }
        public int? modelId { get; set; }
        public string liveApplication { get; set; }
        public int? statusId { get; set; }
        public string remarks { get; set; }
        public string target { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
