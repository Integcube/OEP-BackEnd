using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAFinalImplementation
    {
        [Key]
        public int finalImpId { get; set; }
        public string finalImpTitle { get; set; }
        public int isDeleted { get; set; }
        public decimal? score { get; set; } 
    }
}
