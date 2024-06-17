using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Formats.Asn1;
using System.Globalization;
using WebApplication2.Model;

namespace WebApplication2.Controllers
{
    
    [Route("api/CsvData")]
    [ApiController]
    [Authorize]
    public class CsvDataController : ControllerBase
    {
        private readonly string _csvFilePath = "C:\\Users\\jskel\\Desktop\\Api_SuperStore\\Super_Store_Data.csv";

        [HttpGet]
       
        public IActionResult GetAllData(int page = 1, int pageSize = 200)
        {
            using (var reader = new StreamReader(_csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<SuperStore>()
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

                return Ok(JsonConvert.SerializeObject(records));
            }
        }

    }
}
