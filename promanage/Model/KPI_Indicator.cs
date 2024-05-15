using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class KPI_Indicator
    {
        [Key]
        public int indicatorId { get; set; }
        public string indicatorCode { get; set; }
        public string indicatorTitle { get; set; }
        public int groupId { get; set; }
        public int isParent { get; set; }
        public int isDisplay { get; set; }
        //public decimal weight { get; set; }
        public int isDeleted { get; set; }
    }
}
