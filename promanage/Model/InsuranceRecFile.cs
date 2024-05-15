using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceRecFile
    {
        [Key]
        public int faId { get; set; }
        public int irId { get; set; }
        public string path { get; set; }
        public int isDeleted { get; set; }
        public string fileName { get; set; }
        public int createdBy { get; set; }
    }
}
