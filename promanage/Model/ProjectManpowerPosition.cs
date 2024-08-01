using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{

        [Table("Project_ManpowerPosition")]
        public class ProjectManpowerPosition
        {
            [Key]
            public int id { get; set; }
            public string title { get; set; }
            public int isDeleted { get; set; }
        }

    
}
