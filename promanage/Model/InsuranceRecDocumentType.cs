using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class InsuranceRecDocumentType
    {
        [Key]
        public int documentId { get; set; }
        public string documnetTitle { get; set; }
        public int isDeleted { get; set; }

    }
}
