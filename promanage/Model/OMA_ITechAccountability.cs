using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_ITechAccountability
    {
        [Key]
        public int taId { get; set; }
        public string taTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
