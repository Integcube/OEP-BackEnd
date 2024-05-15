using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILSource
    {
        [Key]
        public int sourceId { get; set; }
        public string sourceTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
