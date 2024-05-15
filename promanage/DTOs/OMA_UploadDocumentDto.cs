using Microsoft.AspNetCore.Http;

namespace ActionTrakingSystem.DTOs
{
    public class OMA_UploadDocumentDto
    {
        public IFormFile file { get; set; }
        public string name { get; set; }
        public string remarks { get; set; }
        public string actionId { get; set; }
        public string siteId { get; set; }
        public string technologyId { get; set; }
        public string userId { get; set; }
    }
    public class OMA_DeleteDocumentDto
    {
        public string name { get; set; }
        public string remarks { get; set; }
        public string path { get; set; }
        public int evidenceId { get; set; }
        public int actionId { get; set; }
        public int siteId { get; set; }
        public int technologyId { get; set; }
    }
    public class OMA_DocumnetListDto
    {
        public int userId { get; set; }
        public int actionId { get; set; }
        public int siteId { get; set; }
        public int technologyId { get; set; }
    }
}
