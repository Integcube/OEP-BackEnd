using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class KPI_SiteInfo
    {
        [Key]
        public int infoId { get; set; }
        public int indicatorId { get; set; }
        public int siteId { get; set; }
        public int applicable { get; set; }
        public string measurementTitle { get; set; }
        public string annualTargetTitle { get; set; }
        public decimal weight { get; set; }
        public string unit { get; set; }
        public decimal? factor { get; set; }
        public string classificationTitle { get; set; }
        public int isDeleted { get; set; }
        public int formulaType { get; set; }

    }
}
