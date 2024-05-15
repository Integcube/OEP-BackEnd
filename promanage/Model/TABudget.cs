using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TABudget
    {
        [Key]
        public int budgetId { get; set; }
        public string budgetName { get; set; }
        public decimal? score { get; set; }
        public int isDeleted { get; set; }
    }
}
