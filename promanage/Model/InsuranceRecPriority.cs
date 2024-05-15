using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceRecPriority
    {
        [Key]
        public int ipId { get; set; }
        public string ipTitle { get; set; }
        public int isDeleted { get; set; }


    }
}
