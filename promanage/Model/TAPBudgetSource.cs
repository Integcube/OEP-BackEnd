using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAPBudgetSource
    {
        [Key]
        public int budgetSourceId { get; set; }
        public string budgetSourceTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
