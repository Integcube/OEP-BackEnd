using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class IATrackingCompany
    {
        [Key]
        public int iatId { get; set; }
        public string iatTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
