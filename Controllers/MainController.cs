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

namespace RiotInfo.com.Controllers
{
    public class MainController : Controller
    {
        static string key = "RGAPI-1c908127-022d-464e-9689-774adc7fd1c5";
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

    }
}