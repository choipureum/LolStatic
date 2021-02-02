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

namespace RiotInfo.com.Controllers
{
    public class MainController : Controller
    {
       static string key = "RGAPI-89e74c99-146f-4ee1-95e1-f0bb19ac435b";
       
        // GET: Main
        public ActionResult Index()
        {
            return View();
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

                //로테이션
                FieldInfo rota = new FieldInfo();
                var rotation = api.Champion.GetChampionRotationAsync(Region.Kr).Result;
                rota.MaxNewPlayerLevel=rotation.MaxNewPlayerLevel;
                rota.rotationChamp = rotation.FreeChampionIds;
                dic.Add("rotation", rota);

                //선호 챔피언
                List<ChampionMastery> favoriteChamp = ChampionMasteries(summoner.Id, latestVersion);
                dic.Add("favoriteChamp", favoriteChamp);

            }
            catch (RiotSharpException ex)
            {
                Console.WriteLine("error");
            }
            return View(dic);
        }
        public List<ChampionMastery> ChampionMasteries(string id,string version)
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
            }
            return champList;
        }
        
    }
}