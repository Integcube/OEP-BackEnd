using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class QF_CheckBoxes
    {
        [Key]
        public int checkBoxId { get; set; }
        public string checkBoxTitle { get; set; }
        public string color { get; set; }
        public int isDeleted { get; set; }
    }
}
