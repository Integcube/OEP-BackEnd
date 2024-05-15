using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class QF_Answers
    {
        [Key]
        public int answerId { get; set; }
        public int userId { get; set; }
        public int questionId { get; set; }
        public int checkBoxId { get; set; }
        public string answerTitle { get; set; }
    }
}
