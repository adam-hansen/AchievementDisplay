using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace AchievementDisplay
{
    public class SteamWebAPI
    {
        private const string APIKey = "9C17942B76F580F4A255AE7CDA207A5A";
        private string getOwnedGamesFormat = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={0}&steamid={1}&format=json";
        public string ryansCharId = "76561198019745749";

        private string getGameStatsFormat = "http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid={0}&key={1}&steamid={2}&format=json";

        public Response GetOwnedGames(string steamId)
        {
            string json = null;
            Response resp = null;
            OwnedGames root = null;
            using(var client = new WebClient())
            {
                json = client.DownloadString(new Uri(string.Format(getOwnedGamesFormat,APIKey,steamId)));
                json = json.Replace("\n","");
                json = json.Replace("\t","");
                json = json.Replace("\"", "");
            }

            if(!string.IsNullOrEmpty(json))
            {
                root = JsonConvert.DeserializeObject<OwnedGames>(json);
            }

            if (root != null)
            {
                resp = root.response;
            }
            return resp;
        }

        public Playerstats GetPlayerStatesByGame(string appId, string steamId)
        {

            string json = null;
            using (var client = new WebClient())
            {
                var uri = new Uri(string.Format(getGameStatsFormat, appId, APIKey, steamId));
                try
                {
                    json = client.DownloadString(uri);
                }
                catch (Exception e)
                {
                    return null;
                }
                json = json.Replace("\n", "");
                json = json.Replace("\t", "");
                //json = json.Replace("\"", "");
            }
            var gameStats = JsonConvert.DeserializeObject<GameStats>(json);

            return gameStats.playerstats;
        }

    }

    //should seperate the json classes but whatever
    [JsonObject(MemberSerialization.OptIn)]
    public class Game
    {
        [JsonProperty]
        public int appid { get; set; }

        [JsonProperty]
        public int playtime_forever { get; set; }

        [JsonProperty]
        public int playtime_2weeks { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Response
    {
        [JsonProperty]
        public int game_count { get; set; }

        [JsonProperty]
        public List<Game> games { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class OwnedGames
    {
        [JsonProperty]
        public Response response { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Achievement
    {
        [JsonProperty]
        public string apiname { get; set; }

        [JsonProperty]
        public int achieved { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Playerstats
    {
        [JsonProperty]
        public string steamID { get; set; }

        [JsonProperty]
        public string gameName { get; set; }

        [JsonProperty]
        public List<Achievement> achievements { get; set; }

        [JsonProperty]
        public bool success { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class GameStats
    {
        [JsonProperty]
        public Playerstats playerstats { get; set; }
    }

    //wrap games for display
    public class GameDisplay
    {
        public string AppId { get; set; }
        public int PossibleAch { get; set; }
        public int ObtainedAch { get; set; }
        public string Name { get; set; }
    }
}
