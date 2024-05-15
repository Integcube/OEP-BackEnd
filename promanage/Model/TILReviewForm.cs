using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILReviewForm
    {
        [Key]
        public int reviewFormId { get; set; }
        public string reviewFormTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
