using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using ActionTrakingSystem.DTOs;

namespace ActionTrakingSystem.Model
{
    public static class ImageServer
    {
        public static string getImage(string path)
        {
            if (path == null)
            {
                return null;
            }
            string result = "";
            var img64Bytes = "";
            string Localurl = "http://localhost:57714/";
            using (var client = new HttpClient())
            {
                string url = Localurl + "GetFilePatientSS.ashx"; // Just a sample url
                WebClient wc = new WebClient();
                wc.QueryString.Add("path", path.ToString());
                result = System.Text.Encoding.UTF8.GetString(wc.UploadValues(url, "POST", wc.QueryString));
                dynamic imgBytes = JsonConvert.DeserializeObject(result);
                img64Bytes = imgBytes["Img"].ToString();
            }
            return img64Bytes;
        }
        public static string saveImage(string imageStr)
        {
            string result = "";
            var picpath = "";
            string Localurl = "http://localhost:57714/";
            var jsonString = JsonConvert.SerializeObject(imageStr);
            using (var client = new HttpClient())
            {
                string files = jsonString;
                string url = Localurl + "UploadFilePatientSS.ashx?vid=" + 1 + "&delStr=ABC";
                WebClient wc = new WebClient();
                wc.QueryString.Add("file", files);
                result = System.Text.Encoding.UTF8.GetString(wc.UploadValues(url, "POST", wc.QueryString));
                dynamic path = JsonConvert.DeserializeObject(result);
                picpath = path["ProfilefilePath"].ToString();
            }
            return picpath;
        }
    }
 
}
