using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using ActionTrakingSystem.Model;
using System.Threading.Tasks;

namespace ActionTrakingSystem.DTOs
{
    public class ProjectPlandDto
    {

        public int planId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
        public int pmId { get; set; }
        public int siteId { get; set; }
        public DateTime startDate { get; set; }
        public int startplanId { get; set; }
        
    }


    public class ProjectPlanPhaseDto
    {
        public string title { get; set; }
        public decimal weightage { get; set; }
        public int duration { get; set; }
        public int planId { get; set; }
        public int phaseId { get; set; }
        public int createdBy { get; set; }
        public int displayOrder { get; set; }
        
    }

    public class ProjectPlanPhase
    {

        public int phaseId { get; set; }
        public string title { get; set; }

    }

    public class ProjectPlanTaskDto
    {
        public int taskId { get; set; }
        public string title { get; set; }
        //public decimal weightage { get; set; }
        public decimal duration { get; set; }
        public int planId { get; set; }
        public int phaseId { get; set; }
        public int createdBy { get; set; }
        public int predecessorId { get; set; }
        public decimal lagDays { get; set; }
        public int idealScore { get; set; }
        public string code { get; set; }
        public int predecessorType { get; set; }
        public int durationUnit { get; set; }
        public int lagUnit { get; set; }
        public int taskdisplayOrder { get; set; }
        

    }
    public class ProjectPlanSubTaskDto
    {
        public int taskId { get; set; }
        public string title { get; set; }
        public decimal duration { get; set; }
        public int planId { get; set; }
        public int phaseId { get; set; }
        public int mainTaskId { get; set; }
        public int createdBy { get; set; }
        public int predecessorId { get; set; }
        public decimal lagDays { get; set; }
        public int idealScore { get; set; }
        public string code { get; set; }
        public int predecessorType { get; set; }
        public int durationUnit { get; set; }
        public int lagUnit { get; set; }
        public int taskdisplayOrder { get; set;}
        
    }



        public class ProjectPlanTaskDtoSp
        {
            public int TaskId { get; set; }
            public string? Title { get; set; }
            public decimal Duration { get; set; }
            public decimal Weightage { get; set; }
            public int PlanId { get; set; }
            public int PhaseId { get; set; }
            public string PhaseTitle { get; set; }
            public int PhaseDuration { get; set; }
            public decimal PhaseWeightage { get; set; }
            public int TaskParentId { get; set; }
            public DateTime CreatedOn { get; set; }
            public int CreatedBy { get; set; }
            public int IsDeleted { get; set; }
            public int? ModifiedBy { get; set; }
            public DateTime? ModifiedOn { get; set; }
            public string? ParentTitle { get; set; }
            public int? Level { get; set; }
            public int IsChild { get; set; }
            public string code { get; set; }
            public decimal? idealScore { get; set; }
            public int? predecessorId { get; set; }
            public decimal? lagDays { get; set; }
           public string predecessorTitle { get; set; }
           public string predecessor { get; set; }
           public int predecessorType { get; set; }
        public string durationWithunit { get; set; }
        public string lagDaysWithunit { get; set; }
        public int durationUnit { get; set; }
        public int lagUnit { get; set; }
    }
    public class AssignTaskDTO
    {
        public int? PhaseId { get; set; }
        public string? PhaseTitle { get; set; }
        public  int? PhaseDuration { get; set; }
        public decimal? PhaseWeightage { get; set; }
        public int? phasePgmId { get; set; }
        public DateTime? phaseStartDate { get; set; }
        public DateTime? phaseEndDate { get; set; }
        public int? startPhaseId { get; set; }
        public int? TaskId { get; set; }
        public string? Title { get; set; }
        public decimal? Duration { get; set; }
        public decimal? Weightage { get; set; }
        public int? PlanId { get; set; }
        public int? TaskParentId { get; set; }
        public string? ParentTitle { get; set; }
        public int? Level { get; set; }
        public int? IsChild { get; set; }
        public int? TaskAssignmentId { get; set; }
        public int? AssignTo { get; set; }
        public DateTime? TaskAssignmentStartDate { get; set; }
        public DateTime? TaskAssignmentEndDate { get; set; }
        public int? PredecessorId { get; set; }
        public int? predecessorType { get; set; }
        public decimal? lagDays { get; set; }
        public string? predecessorTitle { get; set; }
        public string? predecessor { get; set; }
        public string? code { get; set; }
        public int? durationUnit { get; set; }
        public int? lagUnit { get; set; }
        public string? durationWithunit { get; set; }
        public string? lagDaysWithunit { get; set; }
        public decimal? idealScore { get; set; }
        public int? displayOrder { get; set; }
        public int? taskdisplayOrder { get; set; }
        
    }

    public class ProjectStartDTO
    {
        public int PlanId { get; set; }
        public int SiteId { get; set; }
        public DateTime? startDate { get; set; }
        public int Id { get; set; }
        public int pmId { get; set; }


    }

    public class TaskDTO
    {
        public string PhaseTitle { get; set; }
        public DateTime? PhaseStartDate { get; set; }
        public DateTime? PhaseEndDate { get; set; }
        public int PhasePgmId { get; set; }
        public int startPhaseId { get; set; }
        
        public int TaskId { get; set; }
        public int PhaseId { get; set; }
        public string Title { get; set; }
        public decimal Duration { get; set; }
        public DateTime? TaskAssignmentStartDate { get; set; }
        public DateTime? TaskAssignmentEndDate { get; set; }
        public int TaskAssignmentId { get; set; }
        
        public int? AssignTo { get; set; }
        public int IsChild { get; set; }
        public decimal Weightage { get; set; }
       

    }

    public class ProjectPlanDto
    {
        
        public int createdBy { get; set; }
        public ProjectStartDTO Project { get; set; }
        public List<TaskDTO> Tasks { get; set; }
    }


    public class SaveProgress
    {

        public int createdBy { get; set; }
        public List<ProjectExeTask> Tasks { get; set; }
    }

    public class MonthYearDTO
    {
        public int UserId { get; set; }
        public int startPlanId { get; set; }
        public List<string> Months { get; set; }
    }



}
