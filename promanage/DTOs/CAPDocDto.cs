using Microsoft.AspNetCore.Http;

namespace ActionTrakingSystem.DTOs
{
    public class CAPDocDto
    {
        public int docId { get; set; }
        public int siteId { get; set; }
        public string siteTitle { get; set; }
    }
    public class uploadDocDto
    {
        public IFormFile file { get; set; }
        public string docId { get; set; }
        public string userId { get; set; }
        public string siteId { get; set; }
    }
}
