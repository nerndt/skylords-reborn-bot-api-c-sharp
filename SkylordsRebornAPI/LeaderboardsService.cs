using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkylordsRebornAPI.Exceptions;
using SkylordsRebornAPI.Leaderboards;

namespace SkylordsRebornAPI
{
    public class LeaderboardsService
    {
        //private readonly string baseUrl = "https://skylords-reborn-skylords-reborn-api-hub-backend.staging.skylords.eu/"; // "https://leaderboards.backend.skylords.eu";
        private readonly string baseUrl = "https://hub.backend.skylords.eu/api/";

        public ulong? NextCachingRefresh()
        {
            var url = $"{baseUrl}/api/next-load";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";
            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            var value = (JObject.Parse(text).GetValue("in") ?? -1).Value<ulong>();
            return value;
        }

        public List<GenericData> GetMapByPvEMode(PvEModes pveMode)
        {
            var url = $"{baseUrl}api/maps/{(int) pveMode}pve";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";
            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            var deserializeObject = JsonConvert.DeserializeObject<List<GenericData>>(text);
            return deserializeObject;
        }

        public List<GenericData> GetTimeRanges()
        {
            var url = $"{baseUrl}/api/ranges";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";
            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            var deserializeObject = JsonConvert.DeserializeObject<List<GenericData>>(text);
            return deserializeObject;
        }

        public List<GenericData> GetDifficulties()
        {
            var url = $"{baseUrl}/api/difficulties";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";
            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            var deserializeObject = JsonConvert.DeserializeObject<List<GenericData>>(text);
            return deserializeObject;
        }

        public uint GetPvELeaderboardCount(PvEModes pveMode, PlayerCount playerCount, int mapID, int month)
        {
            var url =
                $"{baseUrl}/api/leaderboards/pve-count/{(int) pveMode}/{(int) playerCount}/{mapID}/{month}";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";

            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            return (JObject.Parse(text).GetValue("count") ?? -1).Value<uint>();
        }

        public List<MatchInfo> GetPvELeaderboard(PvEModes pveMode, PlayerCount playerCount, int mapID, int month,
            int page, int totalResults)
        {
            var url =
                $"{baseUrl}/api/leaderboards/pve/{(int) pveMode}/{(int) playerCount}/{mapID}/{month}/{page}/{totalResults}";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";

            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            var deserializeObject = JsonConvert.DeserializeObject<List<MatchInfo>>(text);
            return deserializeObject;
        }

        public uint GetPVPLeaderboardCount(PvPModes pvpMode, int month)
        {
            var url = $"{baseUrl}/api/leaderboards/pvp-count/{(int) pvpMode}/{month}";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";

            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            return (JObject.Parse(text).GetValue("count") ?? -1).Value<uint>();
        }

        public List<PVPMatchInfo> GetPVPLeaderboard(PvPModes pvpMode, int month, int page, int totalResults)
        {
            var url = $"{baseUrl}/api/leaderboards/pvp/{(int) pvpMode}/{month}/{page}/{totalResults}";

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json; charset=utf-8";

            var response = webRequest.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()!))
            {
                text = sr.ReadToEnd();
            }

            CheckBackendStatus(text);
            var deserializeObject = JsonConvert.DeserializeObject<List<PVPMatchInfo>>(text);
            return deserializeObject;
        }

        private void CheckBackendStatus(string responseText)
        {
            var state = JObject.Parse(responseText).GetValue("state");
            if (state != null)
            {
                var stateValue = state.Value<string>();
                throw stateValue switch
                {
                    "invalid" => new BackendInvalidException(),
                    "loading" => new BackendCachingException(),
                    _ => new BackendUnknownException($"stateValue: {stateValue}")
                };
            }
        }
    }
}