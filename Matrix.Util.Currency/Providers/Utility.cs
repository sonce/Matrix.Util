using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Matrix.Util.Currency
{
    public static class Utility
    {
        public static async Task<JObject> GetApiResult(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string strResult = await response.Content.ReadAsStringAsync();
                    var jobj = JsonConvert.DeserializeObject<JObject>(strResult);
                    return jobj;
                }
                else
                    return null;
            }
        }
    }
}
