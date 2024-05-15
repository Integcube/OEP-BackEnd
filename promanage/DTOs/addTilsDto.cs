using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ActionTrakingSystem.DTOs
{
    public class tilDocDto
    {
        public IList<IFormFile> pdfFile;
        public int tilId { get; set; }
        public int userId { get; set; }

    }
    public class getTilUserFilterDto
    {
        public int userId { get; set; }
        public string docTypeList { get; set; }
        public string statusList { get; set; }
        public string formList { get; set; }
        public string focusList { get; set; }
        public string severityList { get; set; }
        public string equipmentList { get; set; }
    }

    public class getTilEvaluationDto
    {
        public int userId { get; set; }
        public string statusList { get; set; }
        public string docTypeList { get; set; }
        public string statusReviewList { get; set; }
        public string formList { get; set; }
        public string focusList { get; set; }
        public string severityList { get; set; }
    }
    public class tilFilterDto
    {
        public DateTime? startDate { get; set; } = null;
        public DateTime? endDate { get; set; } = null;

        public int? documentId { get; set; } = -1;
        public int? statusId { get; set; } = -1;
        public int? formId { get; set; } = -1;

    }
    public class tilUserDto {
        public int userId { get; set; }
        public addTilsDto til { get; set; }
    }
    public class uploadTilFileDto
    {
        public IFormFile tilReport { get; set; }
        public string tilId { get; set; }
        public string userId { get; set; }
    }

    public class addTilsDto
    {
        public int? tilId { get; set; }
        public string tilNumber { get; set; }
        public string alternateNumber { get; set; }
        public string applicabilityNotes { get; set; }
        public string tilTitle { get; set; }
        public string currentRevision { get; set; }
        public int? tilFocusId { get; set; }
        public string tilFocusTitle { get; set; }
        public int? documentTypeId { get; set; }
        public string documentTypeTitle { get; set; }
        public string oem { get; set; }
        public int? oemSeverityId { get; set; }
        public string oemSeverityTitle { get; set; }
        public int? oemTimingId { get; set; }
        public string oemTimingTitle { get; set; }
        public int? reviewForumId { get; set; }
        public string reviewForumtitle { get; set; }
        public string recommendations { get; set; }
        public DateTime? dateReceivedNomac { get; set; }
        public DateTime? dateIssuedDocument { get; set; }
        public int? sourceId { get; set; }
        public string sourceTitle { get; set; }
        public int? reviewStatusId { get; set; }
        public string reviewStatusTitle { get; set; }
        public string notes { get; set; }
        public int? componentId { get; set; }
        public string componentTitle { get; set; }
        public string report { get; set; }
        public int? technicalReviewId { get; set; }
        public string technicalReviewSummary { get; set; }
        public string implementationNotes { get; set; }
        public string yearOfIssue { get; set; }
        public int? tbEquipmentId { get; set; }
    }
}
