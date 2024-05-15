using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class TILComponent
    {
        [Key]
        public int componentId { get; set; }
        public string componentTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
