using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkylordsRebornAPI.Cardbase.Cards;

namespace SkylordsRebornAPI.Cardbase
{
    public class CardService
    {
        private readonly string baseSMJUrl = "https://smj.cards/api/"; // NGE04182024 THIS GETS AN OK RESPONSE FROM THE SERVER // "https://cardbase.skylords.eu/";
        private readonly string baseSkylordsRebornUrl = "https://hub.backend.skylords.eu/api/"; // NGE04182024 THIS GETS AN OK RESPONSE FROM THE SERVER // "https://cardbase.skylords.eu/";
        private string urlContent = null;

        async Task ReadWebPageAsync(string url)
        {
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(url);

            using var reader = new StreamReader(stream);
            urlContent = reader.ReadToEnd();
            // Console.WriteLine(urlContent); // Display the fetched content
        }

        public SMJCard[] GetSMJCardList()
        {
            var url = $"{baseSMJUrl}cards";
            try
            {
                ReadWebPageAsync(url).Wait(); // fills in string urlContent
                SMJCards cards = JsonConvert.DeserializeObject<SMJCards>(urlContent);
                List<SMJCard> cardlist = cards.data;
                return cardlist.ToArray();
            }
            catch (Exception ex) // SMJ.cards/api/cards must have changed the JSON!!!!
            {
                string exMessage = ex.Message;
                return null;
            }
        }

        public SkylordsRebornCard[] GetSkylordsRebornCardList()
        {
            var url = $"{baseSkylordsRebornUrl}auctions/cards?id=all";
            try
            {
                ReadWebPageAsync(url).Wait(); // fills in string urlContent
                List<SkylordsRebornCard> cardlist = JsonConvert.DeserializeObject<List<SkylordsRebornCard>>(urlContent);

                //WebClient client = new WebClient();
                //string json = client.DownloadString(url);

                //SMJCards SMJRoot = JsonConvert.DeserializeObject<SMJCards>(json);
                //List<SMJCard> cardlist = SMJRoot.data;
                return cardlist.ToArray();
            }
            catch (Exception ex) // JSON on website must have changed!!!
            {
                string exMessage = ex.Message;
                return null;
            }
        }

        public SMJCard[] HandleCardRequest(List<Tuple<RequestProperty, string>> requestProperties)
        {
            var url = $"{baseSMJUrl}cards";

            /* // NGE04192024
            for (var index = 0; index < requestProperties.Count; index++)
            {
                var requestProperty = requestProperties[index];
                url +=
                    $"{(index > 0 ? "&" : "")}{Enum.GetName(typeof(RequestProperty), requestProperty.Item1)}={requestProperty.Item2}";
            }
            */

            // Call the method
            ReadWebPageAsync(url).Wait(); // fills in string urlContent
            SMJCards cards = JsonConvert.DeserializeObject<SMJCards>(urlContent);

            // NGE04202024 WebClient client = new WebClient();
            // NGE04202024 string json = client.DownloadString(url);
            // NGE04202024 SMJCards cards = JsonConvert.DeserializeObject<SMJCards>(json);

            
            List<SMJCard> cardlist = cards.data;
            return cardlist.ToArray();

            //dynamic array = JsonConvert.DeserializeObject(json); 
            //if (array != null && array.data != null) {
            //dynamic cardListDynamic = array.data;
            //    foreach (var item in cardListDynamic) // Collect all card information
            //    {
            //        // Console.WriteLine($"{item}");
            //        SMJCard jobjectInstance = (SMJCard) JObject.FromObject(item);
            //        cardlist.Add(item);
            //    }
            //}
            //return cardlist.ToArray();


            //var webRequest = WebRequest.Create(url);
            //webRequest.ContentType = "application/json; charset=utf-8";
            //var response = webRequest.GetResponse();
            //string text;
            //using (var sr = new StreamReader(response.GetResponseStream()!))
            //{
            //    text = sr.ReadToEnd();
            //}

            //var deserializeObject = JsonConvert.DeserializeObject<APIWrap<Card>>(text);
            //if (deserializeObject.Success != true)
            //    throw new Exception(
            //        $"There has been an error with the API.\n{deserializeObject.Exception.Details}");
            //var cards = deserializeObject.Result;
            //return cards;
        }
    }
}