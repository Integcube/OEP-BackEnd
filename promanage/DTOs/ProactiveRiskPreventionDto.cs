using System.Collections.Generic;

namespace ActionTrakingSystem.DTOs
{
    public class ProactiveRiskPreventionDto
    {
        public int proactiveId { get; set; }
        public string proactiveReference { get; set; }
        public string proactivetitle { get; set; }
        public string recommendations { get; set; }
        public string guidelines { get; set; }
        public string auditPreperatoryChecklist { get; set; }
        public int? criticalityId { get; set; }
        public string details { get; set; }
        public string criticalityTitle { get; set; }
        public int? categoryId { get; set; }
        public string categoryTitle { get; set; }
        public int? exposureId { get; set; }
        public string exposureTitle { get; set; }
        public int? sourceId { get; set; }
        public string sourceTitle { get; set; }
        public int? themeId { get; set; }
        public string themeTitle { get; set; }
        public string sites { get; set; }
        public int? approachStatusId { get; set; }
        public string approachStatusTitle { get; set; }
    }
    public class getProactiveObjDto
    {
        public int userId { get; set; }
        public int proactiveId { get; set; }
    }
    public class saveProactiveDto
    {
        public int userId { get; set; }
        public ProactiveObjListDto obj { get; set; }
    }
    public class ProactiveObjListDto
    {
      public ProactiveRiskPreventionDto proactive { get; set; }
      public  List<ProactivePhaseListDto>? projectPhase { get; set; }
    }
    public class ProactivePhaseListDto
    {
        public int projectPhaseId { get; set; }
        public string projectPhaseTitle { get; set; }
    }
}
