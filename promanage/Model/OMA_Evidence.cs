using System;
using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OMA_Evidence
    {
        [Key]
        public int evidenceId { get; set; }
        public int actionId { get; set; }
        public int siteId { get; set; }
        public int technologyId { get; set; }
        public string name { get; set; }
        public string remarks { get; set; }
        public string path { get; set; }
        public DateTime createdOn { get; set; }
        public int createdBy { get; set; }
        public int isDeleted { get; set; }
    }
}
