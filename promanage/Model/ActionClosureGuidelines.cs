using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ActionClosureGuidelines
    {
        [Key]
        public int acId { get; set; }
        public string title { get; set; }
        public string recurrence { get; set; }
        public string description { get; set; }
        public string minimumRequirementsForInProgress { get; set; }
        public string minimumRequirementsForImplemented { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int? modifiedOn { get; set; }
        public DateTime? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
