namespace ActionTrakingSystem.DTOs
{
    public class RegionUserDto
    {
        public SaveRegionDto data { get; set; }
        public int userId { get; set; }
    }
    public class SaveRegionDto
    {
        public RegionsDto region { get; set; }
        public RegionUserList[] userList { get; set; }
    }
    public class RegionUserList
    {
        public int userId { get; set; }
        public string userName { get; set; }
    }
    public class RegionsDto
    {
        public int regionId { get; set; }
        public string title { get; set; }
        public string TILsSummary { get; set; }
        public string insuranceSummary { get; set; }
        public int? executiveDirectorId { get; set; }
    }
    public class SelectedRegionDto
    {
        public int userId { get; set; }
        public int regionId { get; set; }
    }
}
