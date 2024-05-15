using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ActionTrakingSystem.Model
{
    public class SiteProjectStatus
    {
        [Key]
        public int projectStatusId { get; set; }
        public string projectStatusTitle { get; set; }
        public int isDeleted { get; set; }
     
    }
}
