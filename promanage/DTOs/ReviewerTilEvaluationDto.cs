using System;

namespace ActionTrakingSystem.DTOs
{
    public class ReviewerTilEvaluationDto
    {
        public TilReviewDto til { get; set; }
        public int userId { get; set; }
    }
    public class TilReviewDto
    {
        public string alternateNumber { get; set; }
        public string applicabilityNotes { get; set; }
        public int componentId { get; set; }
        public string componentTitle { get; set; }
        public bool critical { get; set; }
        public string currentRevision { get; set; }
        public DateTime dateIssuedDocument { get; set; }
        public DateTime dateReceivedNomac { get; set; }
        public int documentTypeId { get; set; }
        public string documentTypeTitle { get; set; }
        public bool evaluated { get; set; }
        public DateTime? evaluationDate { get; set; }
        public string implementationNotes { get; set; }
        public bool mandatory { get; set; }
        public string notes { get; set; }
        public string oem { get; set; }
        public int oemSeverityId { get; set; }
        public string oemSeverityTitle { get; set; }
        public int oemTimingId { get; set; }
        public string oemTimingTitle { get; set; }
        public string recommendations { get; set; }
        public string report { get; set; }
        public int reviewForumId { get; set; }
        public string reviewForumtitle { get; set; }
        public int reviewStatus { get; set; }
        public int reviewStatusId { get; set; }
        public string reviewStatusTitle { get; set; }
        public string reviewTitle { get; set; }
        public bool safetyCritical { get; set; }
        public int sourceId { get; set; }
        public string sourceTitle { get; set; }
        public int technicalReviewId { get; set; }
        public string technicalReviewSummary { get; set; }
        public int technicalReviewUserId { get; set; }
        public int tilFocusId { get; set; }
        public string tilFocusTitle { get; set; }
        public int tilId { get; set; }
        public string tilNumber { get; set; }
        public string tilTitle { get; set; }
        public string yearOfIssue { get; set; }
    }

}
