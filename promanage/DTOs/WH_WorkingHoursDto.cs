namespace ActionTrakingSystem.DTOs
{
    public class WH_WorkingHoursUserDto
    {
        public int userId { get; set; }
        public WH_WorkingHoursDto filter { get; set; }
    }
    public class WH_WorkingHoursDto
    {
        public int regionId { get; set; }
        public int siteId { get; set; }
        public int equipmentId { get; set; }
    }
}
