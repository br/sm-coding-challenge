using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using sm_coding_challenge.Models;

namespace sm_coding_challenge.Services.DataProvider
{
    public class DataProviderImpl : IDataProvider
    {
        readonly HttpClient _client;
        readonly IDistributedCache _cache;
        readonly ILogger _logger;


        static readonly string _DataCacheName = "PlayerDataStr";

        // dee: injecting HttpClient instead of creating it each time
        public DataProviderImpl(
            HttpClient client,
            IDistributedCache cache,
            ILogger<DataProviderImpl> logger
            )
        {
            _client = client;
            _cache = cache;
            _logger = logger;
        }

        static readonly IReadOnlyDictionary<string, PlayerModel> _playersMap = new Dictionary<string, PlayerModel> { };

        public async Task<DataResponseModel> FetchData()
        {
            var dataInCache = true;
            
            var stringData = await _cache.GetStringAsync(_DataCacheName);

            if (string.IsNullOrWhiteSpace(stringData))
            {
                _logger.LogDebug("dataNotInCache");
                dataInCache = false;
                using var response = await _client.GetAsync("https://gist.githubusercontent.com/RichardD012/a81e0d1730555bc0d8856d1be980c803/raw/3fe73fafadf7e5b699f056e55396282ff45a124b/basic.json");

                // dee : Making sure we have a success code
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("failedToFetchData {0}", response.StatusCode);
                    throw new Exception("failedToFetchData");
                }

                stringData = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                _logger.LogDebug("dataInCache");
            }


            // dee: We always deserialize the data before saving to cache to Ensure we got good data
            var dataResponse = JsonConvert.DeserializeObject<DataResponseModel>(stringData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            if (!dataInCache)
            {
                // dee : This data set is not updated very frequently (once a week), So we cache it for 6 days
                await _cache.SetStringAsync(_DataCacheName, stringData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(6)
                });

                _logger.LogDebug("dataSavedInCache");
            }


            return dataResponse;

        }

        public enum Position { Kicking , Passing , Receiving , Rushing }
        

        public async Task<PlayerModel> GetPlayerById(string id)
        {
            var dataResponse = await FetchData();

            var allPlayers =
                dataResponse.Kicking.Select(player=>new { position = Position.Kicking, player})
                .Concat(dataResponse.Passing.Select(player => new { position = Position.Passing, player }))
                .Concat(dataResponse.Receiving.Select(player => new { position = Position.Receiving, player }))
                .Concat(dataResponse.Rushing.Select(player => new { position = Position.Rushing, player }))
                .ToArray();


            var playersMap = (from p in allPlayers
                     group p by p.player.Id into g
                     select new {id = g.Key, playersAndPosition = g.ToArray() })
                     .ToDictionary(k=>k.id, v=>v.playersAndPosition)
                     
                     ;

            if (playersMap.TryGetValue(id,out var playerAndPos))
            {
                return playerAndPos.First().player;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
