using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_IStatus
    {
        [Key]
        public int statusId { get; set; }
        public string statusTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
