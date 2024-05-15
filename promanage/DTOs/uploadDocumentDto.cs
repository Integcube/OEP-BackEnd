using Microsoft.AspNetCore.Http;

namespace ActionTrakingSystem.DTOs
{
    public class uploadDocumentDto
    {
        public IFormFile file { get; set; }
        public string name { get; set; }
        public string remarks { get; set; }
        public string insurenceActionTrackerId { get; set; }
        public string userId { get; set; }
    }
    public class uploadDocumentTilDto
    {
        public IFormFile file { get; set; }
        public string name { get; set; }
        public string remarks { get; set; }
        public string tapId { get; set; }
        public string equipId { get; set; }
        public string userId { get; set; }
    }
    public class deleteDocumentDto
    {
        public string name { get; set; }
        public string remarks { get; set; }
        public string filePath { get; set; }
        public int id { get; set; }
        public int docId { get; set; }
        

    }
    public class deleteDocumentDtoTil
    {
        public string name { get; set; }
        public string remarks { get; set; }
        public string filePath { get; set; }
        public int tapId { get; set; }
        public int equipId { get; set; }
        public int docId { get; set; }


    }
}
