using ActionTrakingSystem.Controllers;
using ActionTrakingSystem.DTOs;
using ActionTrakingSystem.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace ActionTrakingSystem.Model
{
    public class DAL : DbContext
    {
        public DAL(DbContextOptions<DAL> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TilDetailReportDto>().HasNoKey();
            modelBuilder.Entity<TilActionReportDto>().HasNoKey();
            modelBuilder.Entity<InsuranceRecommendatioSpDto>().HasNoKey();
            modelBuilder.Entity<ProjectPlanTaskDtoSp>().HasNoKey();
            modelBuilder.Entity<AssignTaskDTO>().HasNoKey();
            modelBuilder.Entity<ProjectExeTask>().HasNoKey();
            modelBuilder.Entity<ProjectReadinessDTO>().HasNoKey();
            
            base.OnModelCreating(modelBuilder);
            
        }
        public DbSet<InsuranceRecommendatioSpDto> InsuranceRecommendatioSpDto { get; set; }

        public DbSet<AppUser> AppUser { get; set; }
        
        public DbSet<RegionsExecutiveVp> RegionsExecutiveVp { get; set; }
        public DbSet<CountryExecutiveVp> CountryExecutiveVp { get; set; }
        public DbSet<OT_IStatus> OT_IStatus { get; set; }
        public DbSet<OMA_SiteControl> OMA_SiteControl { get; set; }
        public DbSet<OT_Phase> OT_Phase { get; set; }
        public DbSet<TilDetailReportDto> TilDetailReport { get; set; }
        public DbSet<OT_IActionOwnerUser> OT_IActionOwnerUser { get; set; }
        public DbSet<OT_IActionOwner> OT_IActionOwner { get; set; }
        public DbSet<OT_PhaseOutageTracker> OT_PhaseOutageTracker { get; set; }
        public DbSet<OT_PhaseOutageTrackerFile> OT_PhaseOutageTrackerFile { get; set; }
        public DbSet<OT_PhaseOutageTrackerProgress> OT_PhaseOutageTrackerProgress { get; set; }
        public DbSet<OT_PhaseReadinessDescriptionAO> OT_PhaseReadinessDescriptionAO { get; set; }
        public DbSet<OT_SiteEquipment> OT_SiteEquipment { get; set; }
        public DbSet<KPI_FormulaType> KPI_FormulaType { get; set; }
        public DbSet<WH_ContractOutages> WH_ContractOutages { get; set; }
        public DbSet<Cluster> Cluster { get; set; }
        public DbSet<ClusterExecutiveVp> ClusterExecutiveVp { get; set; }
        public DbSet<OT_PhaseDuration> OT_PhaseDuration { get; set; }
        public DbSet<OT_PhaseReadinessDescription> OT_PhaseReadinessDescription { get; set; }
        public DbSet<OT_ISiteOutages> OT_ISiteOutages { get; set; }
        public DbSet<OT_SiteNextOutages> OT_SiteNextOutages { get; set; }
        public DbSet<TilBulletinEquipment> TilBulletinEquipment { get; set; }

        public DbSet<WH_StartingHours> WH_StartingHours { get; set; }
        public DbSet<TechnicalEvaluationStatus> TechnicalEvaluationStatus { get; set; }

        public DbSet<WH_MonthlyHours> WH_MonthlyHours { get; set; }

        public DbSet<WH_SiteEquipment> WH_SiteEquipment { get; set; }
        public DbSet<WH_ISiteOutages> WH_ISiteOutages { get; set; }
        public DbSet<WH_SiteNextOutages> WH_SiteNextOutages { get; set; }
        public DbSet<TILAccessControl> TILAccessControl { get; set; }
        public DbSet<InsuranceAccessControl> InsuranceAccessControl { get; set; }
        public DbSet<CAP_ModelStatus> CAP_ModelStatus { get; set; }
        public DbSet<CAP_LatestUpdateStatus> CAP_LatestUpdateStatus { get; set; }
        public DbSet<CAP_LatestUpdate> CAP_LatestUpdate { get; set; }
        public DbSet<ModelEquipmentType> ModelEquipmentType { get; set; }
        public DbSet<CAP_Documents> CAP_Documents { get; set; }
        public DbSet<OMA_IPriority> OMA_IPriority { get; set; }
        public DbSet<OMA_IProgram> OMA_IProgram { get; set; }
        public DbSet<OMA_IStatus> OMA_IStatus { get; set; }
        public DbSet<KPI_Indicator> KPI_Indicator { get; set; }
        public DbSet<KPI_SiteList> KPI_SiteList { get; set; }
        public DbSet<KPI_IndicatorGroup> KPI_IndicatorGroup { get; set; }
        public DbSet<KPI_IndicatorWeightage> KPI_IndicatorWeightage { get; set; }
        public DbSet<KPI_UserGroup> KPI_UserGroup { get; set; }
        public DbSet<OMA_ITechAccountability> OMA_ITechAccountability { get; set; }
        public DbSet<OMA_MitigationAction> OMA_MitigationAction { get; set; }
        public DbSet<OMA_MitigationResult> OMA_MitigationResult { get; set; }
        public DbSet<OMA_MitigationActionKeyPhases> OMA_MitigationActionKeyPhases { get; set; }
        public DbSet<OMA_ProgramTechnologies> OMA_ProgramTechnologies { get; set; }
        public DbSet<OMA_Evidence> OMA_Evidence { get; set; }

        public DbSet<TILAccess> TILAccess { get; set; }
        public DbSet<IATrackingDaysStatus> IATrackingDaysStatus { get; set; }
        public DbSet<TILBulletinFile> TILBulletinFile { get; set; }
        public DbSet<TILActiontrackerFile> TILActiontrackerFile { get; set; }
        public DbSet<IATrackingEvidence> IATrackingEvidence { get; set; }
        public DbSet<SitesInsuranceReport> SitesInsuranceReport { get; set; }
        public DbSet<SitesTilReport> SitesTilReport { get; set; }
        public DbSet<CAP_IWSCActions> CAP_IWSCActions { get; set; }
        public DbSet<CAP_IWSCDept> CAP_IWSCDept { get; set; }
        public DbSet<CAP_IWSCPriority> CAP_IWSCPriority { get; set; }
        public DbSet<CAP_IWSCStatus> CAP_IWSCStatus { get; set; }
        public DbSet<CAP_WSCObservation> CAP_WSCObservation { get; set; }
        public DbSet<TAParts> TAParts { get; set; }
        public DbSet<AppMenu> AppMenu { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<AppRole> AppRole { get; set; }
        public DbSet<Technology> Technology { get; set; }
        public DbSet<AURole> AURole { get; set; }
        public DbSet<AUTechnology> AUTechnology { get; set; }
        public DbSet<AUSite> AUSite { get; set; }
        public DbSet<SiteProjectStatus> SiteProjectStatus { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<ModelEquipment> ModelEquipment { get; set; }
        public DbSet<ModelEquipmentOEM> ModelEquipmentOEM { get; set; }
        public DbSet<AppRoleDetail> AppRoleDetail { get; set; }
        public DbSet<Regions> Regions { get; set; }
        public DbSet<Regions2> Regions2 { get; set; }
        public DbSet<OMA_IKeyPhases> OMA_IKeyPhases { get; set; }
        public DbSet<Sites> Sites { get; set; }
        public DbSet<SiteNextOutages> SiteNextOutages { get; set; }
        public DbSet<TILActionTracker> TILActionTracker { get; set; }
        public DbSet<TABudgetSource> TABudgetSource { get; set; }
        public DbSet<TAStatus> TAStatus { get; set; }
        public DbSet<TADayToTarget> TADayToTarget { get; set; }
        public DbSet<TASapPlanning> TASapPlanning { get; set; }
        public DbSet<TAFinalImplementation> TAFinalImplementation { get; set; }
        public DbSet<TAEvidence> TAEvidence { get; set; }
        public DbSet<TABudget> TABudget { get; set; }
        public DbSet<TILActionPackage> TILActionPackage { get; set; }
        public DbSet<ActionClosureGuidelines> ActionClosureGuidelines { get; set; }
        public DbSet<OutageTypes> OutageTypes { get; set; }
        public DbSet<TAPBudgetSource> TAPBudgetSource { get; set; }
        public DbSet<SiteEquipment> SiteEquipment { get; set; }
        public DbSet<ProactiveRiskPrevention> ProactiveRiskPrevention { get; set; }
        //proactives

        public DbSet<ProactiveCriticality> ProactiveCriticality { get; set; }
        public DbSet<ProactiveCategory> ProactiveCategory { get; set; }
        public DbSet<ProactiveExposure> ProactiveExposure { get; set; }
        public DbSet<ProactiveAudit> ProactiveAudit { get; set; }
        public DbSet<ProactiveApproachStatus> ProactiveApproachStatus { get; set; }
        public DbSet<proactiveInsurenceDetail> proactiveInsurenceDetail { get; set; }
        public DbSet<ProactiveProjectPhase> ProactiveProjectPhase { get; set; }
        public DbSet<proactiveProjectPhaseDetail> proactiveProjectPhaseDetail { get; set; }
        public DbSet<ProactiveSource> ProactiveSource { get; set; }
        public DbSet<proactiveTechnologyDetail> proactiveTechnologyDetail { get; set; }
        public DbSet<ProactiveTheme> ProactiveTheme { get; set; }

        //
        public DbSet<InsuranceRecType> InsuranceRecType { get; set; }
        public DbSet<InsuranceRecSource> InsuranceRecSource { get; set; }
        public DbSet<IATrackingFile> IATrackingFile { get; set; }
        public DbSet<InsuranceRecDocumentType> InsuranceRecDocumentType { get; set; }
        public DbSet<InsuranceRecNomacStatus> InsuranceRecNomacStatus { get; set; }
        public DbSet<InsuranceRecPriority> InsuranceRecPriority { get; set; }
        public DbSet<InsurenceActionTracker> InsurenceActionTracker { get; set; }
        public DbSet<IATrackingCompany> IATrackingCompany { get; set; }
        public DbSet<IATrackingSource> IATrackingSource { get; set; }
        public DbSet<IATrackingStatus> IATrackingStatus { get; set; }
        public DbSet<TILBulletin> TILBulletin { get; set; }
        public DbSet<SitesTechnology> SitesTechnology { get; set; }
        public DbSet<TILComponent> TILComponent { get; set; }
        public DbSet<TILDocumentType> TILDocumentType { get; set; }
        public DbSet<TILEquipment> TILEquipment { get; set; }
        public DbSet<TILFocus> TILFocus { get; set; }
        public DbSet<TechnicalEvaluation> TechnicalEvaluation { get; set; }
        public DbSet<TAPEquipment> TAPEquipment { get; set; }
        public DbSet<InsuranceRecFile> InsuranceRecFile { get; set; }
        public DbSet<TILOEMSeverity> TILOEMSeverity { get; set; }
        public DbSet<TILOEMTimimgCode> TILOEMTimimgCode { get; set; }
        public DbSet<TILReviewForm> TILReviewForm { get; set; }
        public DbSet<TILReviewStatus> TILReviewStatus { get; set; }
        public DbSet<TILSource> TILSource { get; set; }
        public DbSet<InsuranceRecInsurenceStatus> InsuranceRecInsurenceStatus { get; set; }
        public DbSet<InsuranceRecommendations> InsuranceRecommendations { get; set; }
        public DbSet<TAPPriority> TAPPriority { get; set; }
        public DbSet<TAPReviewStatus> TAPReviewStatus { get; set; }
        public DbSet<TAPUsers> TAPUsers { get; set; }
        public DbSet<QF_Answers> QF_Answers { get; set; }
        public DbSet<QF_CheckBoxes> QF_CheckBoxes { get; set; }
        public DbSet<QF_Questions> QF_Questions { get; set; }
        public DbSet<QF_User> QF_User { get; set; }
        public DbSet<KPI_SiteInfo> KPI_SiteInfo { get; set; }
        public DbSet<TilActionReportDto> TilActionReportDto { get; set; }

        //Project plan
        public DbSet<ProjectPlan> ProjectPlan { get; set; }
        public DbSet<ProjectPlanTask> ProjectPlanTask { get; set; }
        public DbSet<ProjectPhase> ProjectPhase { get; set; }
        public DbSet<ProjectPlanTaskDtoSp> ProjectPlanTaskDtoSp { get; set; }
        public DbSet<ProjectStartPlan> ProjectStartPlan { get; set; }
        public DbSet<AssignTaskDTO> AssignTaskDTO { get; set; }
        public DbSet<ProjectStartPhase> ProjectStartPhase { get; set; }
        public DbSet<ProjectTaskAssignment> ProjectTaskAssignment { get; set; }
        public DbSet<Project_PredecessorType> Project_PredecessorType { get; set; }
        public DbSet<ProjectKeyIssues> ProjectKeyIssues { get; set; }
        public DbSet<ProjectExeTask> ProjectExeTask { get; set; }
        public DbSet<ProjectManpowerPlanData> ProjectManpowerPlanData { get; set; }
        public DbSet<ProjectManpowerPosition> ProjectManpowerPosition { get; set; }
        public DbSet<ProjectKeyIssuesDocument> ProjectKeyIssuesDocument { get; set; }
        public DbSet<ProjectReadinessPhase> ProjectReadinessPhase { get; set; }
        public DbSet<ProjectReadinessChecklist> ProjectReadinessChecklist { get; set; }
        public DbSet<ProjectReadiness> ProjectReadiness { get; set; }
        public DbSet<ProjectReadinessDTO> ProjectReadinessDTO { get; set; }
        public DbSet<ProjectReadinessDocument> ProjectReadinessDocument { get; set; }
        
    }
}
