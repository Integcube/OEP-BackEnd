using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class proactiveProjectPhaseDetail
    {
        [Key]
        public int pidId { get; set; }
        public int ppId { get; set; }
        public int proactiveId { get; set; }
        public int isDeleted { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
    }
}
