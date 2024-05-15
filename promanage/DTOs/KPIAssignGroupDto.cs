using System;

namespace ActionTrakingSystem.DTOs
{
    public class KPIAssignGroupDto
    {
        public int userId { get; set; }
        public int assignedUserId { get; set; }

    }
    public class KPiAssignGroup 
    {
        public int groupId { get; set; }
        public string groupTitle { get; set; }
        public string groupCode { get; set; }
        public bool selected { get; set; }
    }
    public class SaveAssignGroupDto
    {
        public int userId { get; set; }
        public int assignedUserId { get; set; }
        public KPiAssignGroup[] data { get; set; }
    }
}
