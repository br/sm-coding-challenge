using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using sm_coding_challenge.Models;

namespace sm_coding_challenge.Services.DataProvider
{
    public class DataProviderImpl : IDataProvider
    {
        public PlayerModel GetPlayerById(string id)
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            using (var client = new HttpClient(handler))
            {
                var response = client.GetAsync("https://gist.githubusercontent.com/RichardD012/a81e0d1730555bc0d8856d1be980c803/raw/3fe73fafadf7e5b699f056e55396282ff45a124b/basic.json").Result;
                var stringData = response.Content.ReadAsStringAsync().Result;
                var dataResponse = JsonConvert.DeserializeObject<DataResponseModel>(stringData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                foreach(var player in dataResponse.Rushing)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
                foreach(var player in dataResponse.Passing)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
                foreach(var player in dataResponse.Receiving)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
                foreach(var player in dataResponse.Receiving)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
                foreach(var player in dataResponse.Kicking)
                {
                    if(player.Id.Equals(id))
                    {
                        return player;
                    }
                }
            }
            return null;
        }
    }
}
