namespace ActionTrakingSystem.DTOs
{
    public class OMA_SaveSiteControlDto
    {
        public int siteId { get; set; }
        public int userId { get; set; }
        public OMA_SiteControlDto[] siteControl { get; set; }
    }
    public class OMA_SiteControlDto
    {
        public int techId { get; set; }
        public string technologyTitle { get; set; }
        public int siteId { get; set; }
        public string siteTitle { get; set; }
        public int programId { get; set; }
        public string programTitle { get; set; }
        public int siteControlId { get; set; }
        public bool selected { get; set; }
    }
    public class OMA_GetSiteControlDto
    {
        public int siteId { get; set; }
    }
}
