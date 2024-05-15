using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceRecInsurenceStatus
    {
        [Key]
        public int insurenceStatusId { get; set; }
        public string insurenceStatusTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
