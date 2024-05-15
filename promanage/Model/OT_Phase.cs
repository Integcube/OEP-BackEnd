using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_Phase
    {
        [Key]
        public int phaseId { get; set; }
        public int phaseNumber { get; set; }
        public string phaseTitle { get; set; }
        public string phaseDescription { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
