using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_IWSCStatus
    {
        [Key]
        public int wscStatusId { get; set; }
        public string statusTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
