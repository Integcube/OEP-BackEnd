using System;

namespace ActionTrakingSystem.Model
{
    public class ProjectExeTask
    {
        public int PhaseId { get; set; }
        public string PhaseTitle { get; set; }
        public int PhaseDuration { get; set; }
        public decimal PhaseWeightage { get; set; }
        public DateTime? PhaseStartDate { get; set; }
        public DateTime? PhaseEndDate { get; set; }
        public int StartPhaseId { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; }
        public decimal Duration { get; set; }
        public string DurationWithunit { get; set; }
        public decimal Weightage { get; set; }
        public int PlanId { get; set; }
        public int TaskParentId { get; set; }
        public string ParentTitle { get; set; }
        public int Level { get; set; }
        public int IsChild { get; set; }
        public int TaskAssignmentId { get; set; }
        public int AssignTo { get; set; }
        public DateTime? TaskAssignmentStartDate { get; set; }
        public DateTime? TaskAssignmentEndDate { get; set; }
        public int SiteId { get; set; }
        public int PredecessorId { get; set; }
        public int PredecessorType { get; set; }
        public decimal LagDays { get; set; }
        public string LagDaysWithUnit { get; set; }
        public string PredecessorTitle { get; set; }
        public string Predecessor { get; set; }
        public string Code { get; set; }
        public int DurationUnit { get; set; }
        public int LagUnit { get; set; }
        public string AssignToName { get; set; }
        public decimal Progress { get; set; }
        public decimal TaskCompletionProgress { get; set; }
        public decimal IdealScore { get; set; }
        public decimal CompletionProgress { get; set; }
        public DateTime? MinTaskStartDate { get; set; }
        public DateTime? MaxTaskEndDate { get; set; }
        public decimal? ActualWeightage { get; set; }
        public decimal? TotalCompletePlan { get; set; }
        public int? displayOrder { get; set; }

    }
}
