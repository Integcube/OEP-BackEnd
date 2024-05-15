using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILBulletin
    {
        [Key]
        public int tilId { get; set; }
        public string tilNumber { get; set; }
        public string alternateNumber { get; set; }
        public string tilTitle { get; set; }
        public string oem { get; set; }
        public int? technicalReviewId { get; set; }
        public string recommendations { get; set; }
        public int? oemTimimgCodeId { get; set; }
        public int? oemSeverityId { get; set; }
        public int? focusId { get; set; }
        public DateTime? dateReceivedNomac { get; set; }
        public DateTime? dateIssuedDocument { get; set; }
        public string currentRevision { get; set; }
        public int? documentTypeId { get; set; }
        public int? sourceId { get; set; }
        public int? reviewStatusId { get; set; }
        public string notes { get; set; }
        public int? reviewForumId { get; set; }
        public int? componentId { get; set; }
        public string applicabilityNotes { get; set; }
        public string report { get; set; }
        public string yearOfIssue { get; set; }
        public string implementationNotes { get; set; }
        public int isDeleted { get; set; }
        public int? tbEquipmentId { get; set; }
        public int? createdBy { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? createdOn { get; set; }
        public DateTime? modifiedOn { get; set; }

    }
}
