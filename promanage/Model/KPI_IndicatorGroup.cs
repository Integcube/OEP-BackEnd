using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class KPI_IndicatorGroup
    {
        [Key]
        public int groupId { get; set; }
        public string groupTitle { get; set; }
        public string groupCode { get; set; }
        public decimal weight { get; set; }
        public int isDeleted { get; set; }
        public string color { get; set; }
    }
}
