using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RiotInfo.com.Util;
using RiotSharp;
using RiotSharp.Misc;
using RiotInfo.com.Bll;
using Newtonsoft.Json;
using System.Dynamic;

namespace RiotInfo.com.Controllers
{
    public class MainController : Controller
    {
        static string key = "riotAPI";
        private readonly FieldBiz _FieldBiz = new FieldBiz();
        // GET: Main
        public ActionResult Index()
        {
            var api = RiotApi.GetDevelopmentInstance(key);
            var allVersion = api.StaticData.Versions.GetAllAsync().Result;
            var latestVersion = allVersion[0]; // Example of version: "10.23.1"
            FieldInfo rota = new FieldInfo();
            //rota = GetRotationChamp(latestVersion);
            return View(rota);
        }
        public ActionResult Summoner(String search_summoner)
        {
            var api = RiotApi.GetDevelopmentInstance(key);
            var allVersion = api.StaticData.Versions.GetAllAsync().Result;
            var latestVersion = allVersion[0]; // Example of version: "10.23.1"
            var champions = api.StaticData.Champions.GetAllAsync(latestVersion).Result.Champions.Values;           
            Dictionary<string,Object> dic = new Dictionary<string,Object>();
            
            try
            {
                SummonerInfo info = new SummonerInfo();
                //소환사
                var summoner = api.Summoner.GetSummonerByNameAsync(Region.Kr, search_summoner).Result;
               
                info.name = summoner.Name;
                info.level = (int)summoner.Level;
                info.accountId = summoner.AccountId;
                dic.Add("summoner", info);

               
                //선호 챔피언
                List<ChampionMastery> favoriteChamp = GetChampionMasteries(summoner.Id, latestVersion);
                dic.Add("favoriteChamp", favoriteChamp);

                //티어정보
                List<ExpandoObject> tier =GetTier(summoner.Id);
                dic.Add("tierInfo", tier);
            }
            catch (RiotSharpException ex)
            {
                Console.WriteLine("error");
            }            
            return View(dic);
        }
        public List<ChampionMastery> GetChampionMasteries(string id,string version)
        {            
            var api = RiotApi.GetDevelopmentInstance(key);
            List<ChampionMastery> champList = new List<ChampionMastery>();
            var championMasteries = api.ChampionMastery.GetChampionMasteriesAsync(Region.Kr, id).Result;
            foreach (var championMastery in championMasteries)
            {
                ChampionMastery champ = new ChampionMastery();
                champ.id = championMastery.ChampionId;
                champ.name = api.StaticData.Champions.GetAllAsync(version).Result.Champions.Values.Single(x => x.Id == champ.id).Name; //version 11.2.1
                champ.level = championMastery.ChampionLevel;
                champ.points = championMastery.ChampionPoints;                
                champList.Add(champ);
                if (champList.Count == 5)
                {
                    break;
                }
            }
            return champList;
        }
        public FieldInfo GetRotationChamp(string version)
        {
            var api = RiotApi.GetDevelopmentInstance(key);
            var allVersion = api.StaticData.Versions.GetAllAsync().Result;
            var latestVersion = allVersion[0]; // Example of version: "10.23.1"
            //로테이션
            FieldInfo rota = new FieldInfo();
            var rotation = api.Champion.GetChampionRotationAsync(Region.Kr).Result;
            rota.MaxNewPlayerLevel = rotation.MaxNewPlayerLevel;
            rota.rotationChamp = rotation.FreeChampionIds;         
            
            for (int i = 0; i < rota.rotationChamp.Count; i++)
            {
                Champion champ = _FieldBiz.RetrieveChampion(rota.rotationChamp[i]);
                rota.champions.Add(champ);              
            }
            return rota;

        }
        /// <summary>
        /// 랭크 정보
        /// </summary>
        public static dynamic GetTier(string id)
        {
            Object obj = null;     
            //객체 JSON받아오기
            try
            {
                HttpWebRequest objWRequest = (HttpWebRequest)System.Net.WebRequest.Create("https://kr.api.riotgames.com/lol/league/v4/entries/by-summoner/"+id+"?api_key="+key);
                HttpWebResponse objWResponse = (HttpWebResponse)objWRequest.GetResponse();
                Stream stream = objWResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();
                stream.Close();
                objWResponse.Close();
                //역직렬화

                obj = JsonConvert.DeserializeObject(result);
                
            }
            catch (Exception) { }
            if (obj is string)

            {
                return obj as string;
            }
            else
            {
                return GetDynamicObject(obj as JToken);
            }

        }
        private static dynamic GetDynamicObject(JToken token)
        {
            if (token is JValue)
            {
                return (token as JValue).Value;
            }

            else if (token is JObject)
            {
                ExpandoObject expandoObject = new ExpandoObject();
                (from childToken in token where childToken is JProperty select childToken as JProperty).ToList().ForEach
                (
                    property =>
                    {
                        ((IDictionary<string, object>)expandoObject).Add(property.Name, GetDynamicObject(property.Value));
                    }
                );
                return expandoObject;

            }
            else if (token is JArray)
            {
                List<ExpandoObject> list = new List<ExpandoObject>();

                foreach (JToken item in token as JArray)
                {
                    list.Add(GetDynamicObject(item));
                }
                return list;
            }
            return null;
        }

    }
}
