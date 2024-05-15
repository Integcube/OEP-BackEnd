using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TAPReviewStatus
    {
        [Key]
        public int reviewStatusId { get; set; }
        public string reviewStatusTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
