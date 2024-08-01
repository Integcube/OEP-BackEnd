using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionTrakingSystem.Model
{
    [Table("Project_PredecessorType")]
    public class Project_PredecessorType
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public int isDeleted { get; set; }
        public string code { get; set; }
    }
}
