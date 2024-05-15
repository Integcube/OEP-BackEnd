using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILActionPackage
    {
        [Key]
        public int packageId { get; set; }
        public int? tilId { get; set; }
        public string actionTitle { get; set; }
        public int? actionClosureGuidelinesId { get; set; }
        public int? outageId { get; set; }
        public string actionDescription { get; set; }
        public string expectedBudget { get; set; }
        public int? budgetSourceId { get; set; }
        public string patComments { get; set; }
        public int? priorityId { get; set; }
        public string recurrence { get; set; }
        public int? reviewStatusId { get; set; }
        public DateTime createdOn { get; set; }
        public int? statusId { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }
    }
}
