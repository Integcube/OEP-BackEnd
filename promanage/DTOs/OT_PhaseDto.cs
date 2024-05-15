using ActionTrakingSystem.Model;

namespace ActionTrakingSystem.DTOs
{
    public class OT_PhaseDto
    {
        public int phaseId { get; set; }
        public int phaseNumber { get; set; }
        public string phaseTitle { get; set; }
        public string phaseDescription { get; set; }
    }
    public class GetOT_DurationDto
    {
        public int userId { get; set; }
        public int phaseId { get; set; }
    }
    public class GetSelectedSiteOwner
    {
        public int userId { get; set; }
        public int phaseReadId { get; set; }
    }
    public class GetSelectedOutagesDto
    {
        public int userId { get; set; }
        public int phaseId { get; set; }
    }
    public class SaveOT_PhaseUserDto
    {
        public int userId { get; set; }
        public OT_PhaseDto phase { get; set; }
    }
    public class SaveOT_PhaseDurationUserDto
    {
        public int userId { get; set; }
        public OT_PhaseDurationDto[] PhaseDuration { get; set; }
    }
    public class OT_PhaseDurationDto
    {
        public int phaseId { get; set; }
        public int phaseDurId { get; set; }
        public int outageId { get; set; }
        public string outageTitle { get; set; }
        public decimal durationMonths { get; set; }
    }
    public class OT_GetApiData
    {
        public OT_ActionOwner[] ownerList { get; set; }
        public OT_PhaseDescDto outageReadDesc { get; set; }

    }
    public class OT_ActionOwner
    {
        public int actionOwnerId { get; set; }
        public string actionOwnerTitle { get; set; }

    }
    public class SaveOT_PhaseDescUserDto
    {
        public int userId { get; set; }
        public OT_GetApiData PhaseDesc { get; set; }
    }
    public class DeleteOT_PhaseDescUserDto
    {
        public int userId { get; set; }
        public OT_PhaseDescDto PhaseDesc { get; set; }
    }
    public class OT_PhaseDescDto
    {
        public int phaseId { get; set; }
        public int phaseReadId { get; set; }
        public string phaseReadDesc { get; set; }
        public int phaseReadNum { get; set; }
    }
}
