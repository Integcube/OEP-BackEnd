using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_ModelStatus
    {
        [Key]
        public int modelId { get; set; }
        public string status { get; set; }
        public int isDeleted { get; set; }
    }
}
