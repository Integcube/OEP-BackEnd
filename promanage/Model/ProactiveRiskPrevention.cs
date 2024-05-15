using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ProactiveRiskPrevention
    {
        [Key]
        public int proactiveId { get; set; }
        public string proactivetitle { get; set; }
        public string proactiveReference { get; set; }
        public int? criticalityId { get; set; }
        public int? categoryId { get; set; }
        public int? exposureId { get; set; }
        public string recommendations { get; set; }
        public string guidelines { get; set; }
        public string details { get; set; }
        public int? approachStatusId { get; set; }
        public int? expertId { get; set; }
        public int? sourceId { get; set; }
        public int? themeId { get; set; }
        public string auditPreperatoryChecklist { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
        public int isDeleted { get; set; }

    }
}
