using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FileSync.Controllers
{
    [Route("Places")]
    public class PlacesController : ApiController
    {
        private const string API_KEY = "AIzaSyAYUGBCYTWhJxY-xQ4uP3Aiwo2UdhEMSHk";

        [HttpGet]
        [Route("Places/{searchText}")]
        public async Task<IHttpActionResult> Get(string searchText)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&types=geocode&key={1}",
                HttpUtility.UrlEncode(searchText), HttpUtility.UrlEncode(API_KEY)));
            var content = await response.Content.ReadAsStringAsync();

            var results = new JArray();
            dynamic googleResults;

            var serializer = new JsonSerializer();
            using (var reader = new JsonTextReader(new StringReader(content)))
            {
                googleResults = serializer.Deserialize(reader) as dynamic;
            }

            var predictions = googleResults.predictions;
            foreach (var prediction in predictions)
            {
                var placeName = prediction.description;
                results.Add(placeName);
            }
            return Json(results);
        }
    }
}
