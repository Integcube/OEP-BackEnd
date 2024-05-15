using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class QF_Questions
    {
        [Key]
        public int questionId { get; set; }
        public string questionTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
