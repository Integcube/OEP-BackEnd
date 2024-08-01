using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ActionTrakingSystem.Model
{
   
        [Table("Project_ReadinessDocuments")]
        public class ProjectReadinessDocument
    {
            [Key]
           public int DocId { get; set; }
           public int SiteId { get; set; }
            public DateTime Date { get; set; }
            public string Remarks { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }
            public int UserId { get; set; }
            public int IsDeleted { get; set; }
            public int TypeId { get; set; }
        public int TaskId { get; set; }
    }
    }


