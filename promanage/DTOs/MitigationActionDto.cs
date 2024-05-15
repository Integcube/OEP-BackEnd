using System;

namespace ActionTrakingSystem.DTOs
{


    public class MitigationActionObj
    {
        public keyPhaseDto[] keyPhase { get; set; }
        public MitigationActionDto mitigationAction { get; set; }
    }
    public class keyPhaseDto
    {
        public int keyPhaseId { get; set; }
        public string keyPhaseTitle { get; set; }
        public string keyPhaseCode { get; set; }
    }
    public class MititgationDTODelete
    {
        public int userId { get; set; }
        public MitigationActionDto mitigationAction { get; set; }
    }
    public class MitigationActionUserDto
    {
        public int userId { get; set; }
        public MitigationActionObj  mitigationActionObj{ get; set; }
    }
    public class MitigationResultUserIDDto
    {
        public int userId { get; set; } = -1;
        public MitigationFilterDto filter { get; set; }
    }
    public class MitigationFilterDto
    {
        public int priority { get; set; } = -1;
        public int site { get; set; } = -1;
        public int technology { get; set; } = -1;
        public int status { get; set; } = -1;
        public int program { get; set; } = -1;
    }

    public class MitigationActionDto
    {
        public int actionId { get; set; }
        public string actionTitle { get; set; }
        public int? priorityId { get; set; }
        public string priorityTitle { get; set; }
        public int programId { get; set; }
        public string programTitle { get; set; }
        public string comments { get; set; }
        public string objectiveOutcome { get; set; }
        public int? techAccountabilityId { get; set; }
        public string taTitle { get; set; }
        public DateTime? targetDate { get; set; }
    }
    public class editMitigationDto
    {
        public int userId { get; set; }
        public int actionId { get; set; }
    }
    public class mitigationResultDto
    {
        public int programId { get; set; }
        public string programTitle { get; set; }
        public int actionId { get; set; }
        public string actionTitle { get; set; }
        public int priorityId { get; set; }
        public string priorityTitle { get; set; }
        public int? techAccountabilityId { get; set; }
        public string taTitle { get; set; }
        public string comments { get; set; }
        public string objectiveOutcome { get; set; }
        public DateTime? targetDate { get; set; }
        public int resultId { get; set; }
        public int siteId { get; set; }
        public string siteTitle { get; set; }
        public int technologyId { get; set; }
        public string technologyTitle { get; set; }
        public bool? tataInvolvement { get; set; }
        public string thirdPartyInterface { get; set; }
        public int? statusId { get; set; }
        public string statusTitle { get; set; }
        public string reviewerComment { get; set; }
        public bool? isReviewed { get; set; }
        public string actionComment { get; set; }
        public bool? rework { get; set; }
    }
    public class mitigationResultUserDto
    {
        public int userId { get; set; }
        public mitigationResultDto mitigationResult { get; set; }
    }
    public class programUserDto
    {
        public int userId { get; set; } = -1;
        public int programId { get; set; } = -1;
    }
    public class programDto
    {
        public int programId { get; set; }
        public string programTitle { get; set; }
    }
    public class technologyDto
    {
        public int technologyId { get; set; }
        public string technologyTitle { get; set; }
    }
    public class saveProgramObj
    {
        public programDto program { get; set;}
        public technologyDto[] technologiesSubmit { get; set;}
    }
    public class saveProgramDto
    {
        public saveProgramObj program { get; set; } 
        public int userId { get; set; } 
    }
}
