using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionTrakingSystem.Model
{

    [Table("Project_KeyIssues")]
    public class ProjectKeyIssues
    {

        [Key]
        public int id { get; set; }
        public int siteId { get; set; }
        public DateTime month { get; set; }
        public string hsseIssues { get; set; }
        public string technicalIssues { get; set; }
        public string recruitmentIssues { get; set; }
        public string financialCommercial { get; set; }

        public string roschecklistprogress { get; set; }
        public string technicalriskregister { get; set; }
        public string lessonslearned { get; set; }
        public string previousactioncall { get; set; }
        public int receiveddocument { get; set; }
        public int revieweddocument { get; set; }
  
        public int? createdBy { get; set; }
        public DateTime? createdOn { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int? modifiedBy { get; set; }
    }
}
