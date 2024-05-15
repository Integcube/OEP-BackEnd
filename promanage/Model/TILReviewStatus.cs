using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILReviewStatus
    {
        [Key]
        public int reviewStatusId { get; set; }
        public string reviewStatusTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
