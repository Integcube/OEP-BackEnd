using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILFocus
    {
        [Key]
        public int focusId { get; set; }
        public string focusTitle { get; set; }

        public int isDeleted { get; set; }

    }
}
