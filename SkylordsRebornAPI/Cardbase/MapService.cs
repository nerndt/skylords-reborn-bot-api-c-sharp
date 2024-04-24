using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using SkylordsRebornAPI.Cardbase.Maps;

namespace SkylordsRebornAPI.Cardbase
{
    public class MapService
    {
        // private readonly string baseUrl = "https://skylords-reborn-skylords-reborn-api-hub-backend.staging.skylords.eu/"; // "https://cardbase.skylords.eu/";
        private readonly string baseUrl = "https://hub.backend.skylords.eu/api/"; 

        /// <summary>
        ///     Does not work due to the API not displaying any results.
        /// </summary>
        /// <param name="requestProperties"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Map[] HandleMapRequest(List<Tuple<RequestProperty, string>> requestProperties)
        {
            var url = $"{baseUrl}Cards/GetCards?";
            for (var index = 0; index < requestProperties.Count; index++)
            {
                var requestProperty = requestProperties[index];
                url +=
                    $"{(index > 0 ? "&" : "")}{Enum.GetName(typeof(RequestProperty), requestProperty.Item1)}={requestProperty.Item2}";
            }

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";
            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            var deserializeObject = JsonConvert.DeserializeObject<APIWrap<Map>>(text);
            if (deserializeObject.Success != true)
                throw new Exception(
                    $"There has been an error with the API.\n{deserializeObject.Exception.Details}");
            var maps = deserializeObject.Result;
            return maps;
        }
    }
}