using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class WH_ISiteOutages
    {

        [Key]
        public int outageId { get; set; }
        public string outageTitle { get; set; }
        public int isDeleted { get; set; }
        public string colorCode { get; set; }
    }
}
