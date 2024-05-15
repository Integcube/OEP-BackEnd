using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OT_ISiteOutages
    {
        [Key]
        public int outageId { get; set; }
        public string outageTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
