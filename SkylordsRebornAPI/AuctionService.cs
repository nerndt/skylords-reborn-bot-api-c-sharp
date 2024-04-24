using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkylordsRebornAPI.Auction;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace SkylordsRebornAPI
{
    public class AuctionService
    {
        // private readonly string baseUrl = "https://skylords-reborn-skylords-reborn-api-hub-backend.staging.skylords.eu/api/"; // "https://auctions.backend.skylords.eu/";
        private readonly string baseUrl = "https://hub.backend.skylords.eu/";
        private string urlContent = "";
        async Task ReadWebPageAsync(string url)
        {
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(url);

            using var reader = new StreamReader(stream);
            urlContent = reader.ReadToEnd();
            // Console.WriteLine(urlContent); // Display the fetched content
        }

        public AuctionEntry GetAuctionEntryInfo(int auctionID)
        {
            var url = $"{baseUrl}api/auction/{auctionID}";

            ReadWebPageAsync(url).Wait(); // fills in string urlContent
            if (urlContent == string.Empty) return null;
            return JsonConvert.DeserializeObject<AuctionEntry>(urlContent);

            //var webRequest = WebRequest.Create(url);
            //webRequest.ContentType = "application/json; charset=utf-8";

            //var response = webRequest.GetResponse();
            //string text;
            //using (var sr = new StreamReader(response.GetResponseStream()!))
            //{
            //    text = sr.ReadToEnd();
            //}

            //if (text == string.Empty) return null;
            //return JsonConvert.DeserializeObject<AuctionEntry>(text);
        }

        public List<AuctionEntry> GetAuctionEntriesOfPage(int page, int number, RequestBody requestBody)
        {
            var url = $"{baseUrl}api/auctions/?{page}&{number}";

            ReadWebPageAsync(url).Wait(); // fills in string urlContent
            if (urlContent == string.Empty) return null;
            return JsonConvert.DeserializeObject<List<AuctionEntry>>(urlContent);
            
            //var webRequest = WebRequest.Create(url);
            //webRequest.Method = "POST";
            //webRequest.ContentType = "application/json; charset=utf-8";
            //var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBody));
            //webRequest.ContentLength = data.Length;

            //using (var requestStream = webRequest.GetRequestStream())
            //{
            //    requestStream.Write(data, 0, data.Length);
            //}

            //var response = webRequest.GetResponse();
            //string text;
            //using (var sr = new StreamReader(response.GetResponseStream()!))
            //{
            //    text = sr.ReadToEnd();
            //}

            //if (text == string.Empty) return null;
            //return JsonConvert.DeserializeObject<List<AuctionEntry>>(text);
        }

        public uint GetAmountOfAuctions(RequestBody requestBody)
        {
            // https://hub.backend.skylords.eu/api/auctions/count?cardName=Shaman&min=0&max=1000
            var url = $"{baseUrl}api/auctions/count";
            string urlString = string.Format("{0}?cardName={1}&min={2}&max={3}", url, requestBody.Input, requestBody.Min.ToString(), requestBody.Max.ToString());
            ReadWebPageAsync(urlString).Wait(); // fills in string urlContent
            if (urlContent == string.Empty) return 0;
            return (JObject.Parse(urlContent).GetValue("count") ?? -1).Value<uint>();

            //var webRequest = WebRequest.Create(url);
            //webRequest.Method = "POST";
            //webRequest.ContentType = "application/json; charset=utf-8";
            //var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBody));
            //webRequest.ContentLength = data.Length;

            //using (var requestStream = webRequest.GetRequestStream())
            //{
            //    requestStream.Write(data, 0, data.Length);
            //}

            //var response = webRequest.GetResponse();
            //string text;
            //using (var sr = new StreamReader(response.GetResponseStream()!))
            //{
            //    text = sr.ReadToEnd();
            //}

            //return (JObject.Parse(text).GetValue("count") ?? -1).Value<uint>();
        }

        public AuctionCardIDRequestFormat GetCardInfo(CardId id)
        {
            var url = $"{baseUrl}api/cards/{(int) id}";

            ReadWebPageAsync(url).Wait(); // fills in string urlContent
            if (urlContent == string.Empty) return null;
            if (urlContent == "INVALID") return null;
            var deserializeObject = JsonConvert.DeserializeObject<AuctionCardIDRequestFormat>(urlContent);
            return deserializeObject;

            //var webRequest = WebRequest.Create(url);
            //webRequest.ContentType = "application/json; charset=utf-8";
            //var response = webRequest.GetResponse();
            //string text;
            //using (var sr = new StreamReader(response.GetResponseStream()!))
            //{
            //    text = sr.ReadToEnd();
            //}

            //if (text == "INVALID") return null;
            //var deserializeObject = JsonConvert.DeserializeObject<AuctionCardIDRequestFormat>(text);
            //return deserializeObject;
        }
    }
}