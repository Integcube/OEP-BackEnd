using System;

namespace ActionTrakingSystem.DTOs
{
    public class QF_UserDto
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    public class QF_FormDto
    {
        public int questionId { get; set; }
        public string questionTitle { get; set; }
        public string answer { get; set; }
        public QF_CheckBoxes[] checkBoxes { get; set; }

    }
    public class QF_CheckBoxes
    {
        public Boolean selected { get; set; }
        public string color { get; set; }
        public int checkBoxId { get; set; }
        public string checkBoxTitle { get; set; }
    }
    public class QF_SaveDataDto
    {
        public QF_UserDto user { get; set; }
        public QF_FormDto[] form { get; set; }
    }
}
