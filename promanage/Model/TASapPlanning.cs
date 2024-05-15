using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TASapPlanning
    {
        [Key]
        public int sapPlanningId { get; set; }
        public string sapPlanningTitle { get; set; }
        public int isDeleted { get; set; }
        public decimal? score
        {
            get; set;
        }

    }
}
