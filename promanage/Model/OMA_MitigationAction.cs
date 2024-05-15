using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_MitigationAction
    {
        [Key]
        public int actionId { get; set; }
        public string actionTitle { get; set; }
        public int? priorityId { get; set; }
        public int? programId { get; set; }
        public int? techAccountabilityId { get; set; }
        public string comments { get; set; }
        public string objectiveOutcome { get; set; }
        //public DateTime? targetDate { get; set; }
        public int createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int isDeleted { get; set; }
    }
}
