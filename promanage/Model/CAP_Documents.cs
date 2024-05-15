using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class CAP_Documents
    {
        [Key]
        public int docId { get; set; }
        public int siteId { get; set; }
        public string filePath { get; set; }
        public string fileName { get; set; }
        public int isDeleted { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
    }
}
