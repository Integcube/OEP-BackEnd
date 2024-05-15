using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class OutageTypes
    {
        [Key]
        public int outageTypeId { get; set; }
        public string title { get; set; }
        public int levelOutage { get; set; }
        public string alsoCalled { get; set; }
        public int isDeleted { get; set; }
        public string standardScopeOfWork { get; set; }
        public string normalRecurrence { get; set; }
        public string normalOutageDuration { get; set; }

    }
}
