using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceRecType
    {
        [Key]
        public int typeId { get; set; }
        public string typeTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
