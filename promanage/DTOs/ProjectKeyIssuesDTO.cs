using Microsoft.AspNetCore.Http;
using System;

namespace ActionTrakingSystem.DTOs
{
    public class ProjectKeyIssuesDTO
    {

        public DateTime date { get; set; }
        public int siteId { get; set; }
        public int id { get; set; }
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
        public int userId { get; set; }
        
    }

    public class DocInfo
    {

        public string Date { get; set; }
        public int SiteId { get; set; }
        public int type { get; set; }


    }

    public class UploadFileDto
    {
        public IFormFile? report { get; set; }
        public string siteId { get; set; }
        public string date { get; set; }
        public string remarks { get; set; }
        public string fileName { get; set; }
        public string userId { get; set; }
        public string type { get; set; }
        
    }


}
