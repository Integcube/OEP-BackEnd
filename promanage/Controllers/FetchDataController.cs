using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.X86;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ActionTrakingSystem.Controllers
{

    public class FetchDataController : BaseAPIController
    {
        private readonly IConfiguration _configuration;

        public FetchDataController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetData1")]
        public string GetData()
        {
            return JsonConvert.SerializeObject(new[] { "value1", "value2" });
        }

        [HttpGet("GetData")]
        public IActionResult Get(string type, string query, string key, string param)
        {
            if (query.ToUpper().Contains("INSERT") || query.ToUpper().Contains("DELETE") || query.ToUpper().Contains("UPDATE"))
                return null;

            int isKeyFound = 0;
            string conStr = _configuration.GetConnectionString("Db");
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM QEKey WHERE SecretKey = @Key", con))
                {
                    cmd.Parameters.AddWithValue("@Key", key);
                    isKeyFound = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (isKeyFound == 0)
                    return null;

                type = type.ToUpper();
                string script;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;

                    if (type == "C")
                    {
                        cmd.CommandText = "SELECT Script FROM QEScript WHERE Code = @Query";
                        cmd.Parameters.AddWithValue("@Query", query);
                        script = (string)cmd.ExecuteScalar();
                    }
                    else if (type == "I")
                    {
                        cmd.CommandText = "SELECT Script FROM QESCRIPT WHERE ScriptId = @Query";
                        cmd.Parameters.AddWithValue("@Query", query);
                        script = (string)cmd.ExecuteScalar();
                    }
                    else
                    {
                        return null;
                    }

                    if (!string.IsNullOrEmpty(param))
                    {
                        script = string.Format(script, param.Split(','));
                    }

                    if (_configuration["allowDirectQuery"] == "0" && !(type == "C" || type == "I"))
                    {
                        return null;
                    }

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(script, con))
                    {
                        da.Fill(dt);
                    }

                    return Content(ConvertDataTableToJson(dt), "application/json");

                    // return  ConvertDataTableToJson(dt);
                }
            }
        }

        [HttpGet]
        public object Get(string type, string query, string key)
        {
            return Get(type, query, key, "");
        }

        private static string ConvertDataTableToJSONString(DataTable selDT)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            foreach (DataRow dr in selDT.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();

                foreach (DataColumn col in selDT.Columns)
                {
                    row[col.ColumnName] = dr[col];
                }

                rows.Add(row);
            }

            return JsonConvert.SerializeObject(rows);
        }
        //public string DataTableToJSONWithJSONNet(DataTable table)
        //{
        //    string JSONString = string.Empty;
        //    JSONString = JSONConvert.SerializeObject(table);
        //    return JSONString;
        //}

        public string ConvertDataTableToJson(DataTable dataTable)
        {
            try
            {
                string jsonString = string.Empty;

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    jsonString = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                }

                return jsonString;
            }
            catch (Exception e)
            {
                return (e.Message);
            }
            
        }






    }
}
