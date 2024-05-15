using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class KPI_SiteList
    {
        [Key]
        public int id { get; set; }
        public int KPISiteId { get; set; }
        public int HSEId { get; set; }
        public int CoEId { get; set; }
        public int isDeleted { get; set; }
    }
}
