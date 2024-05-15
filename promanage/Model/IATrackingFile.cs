using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class IATrackingFile
    {
        [Key]
        public int iatFileId { get; set; }
        public int iatId { get; set; }
        public string path { get; set; }
        public string remarks { get; set; }
        public int isDeleted { get; set; }
        public string fileName { get; set; }
        public int createdBy { get; set; }
    }
}
