using RiotInfo.com.Bll;
using RiotInfo.com.Util;
using RiotSharp;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RiotInfo.com.Controllers
{
    public class DbApiController : ApiController
    {
        static string key = "RGAPI-1c908127-022d-464e-9689-774adc7fd1c5";
        private readonly FieldBiz _FieldBiz = new FieldBiz();
        
        [HttpGet]
        [Route("Champ/Upload")]
        public int UploadChamp()
        {
            int result = 0;
            var api = RiotApi.GetDevelopmentInstance(key);
            var allVersion = api.StaticData.Versions.GetAllAsync().Result;
            var latestVersion = allVersion[0]; // Example of version: "10.23.1"
         
            var champ = api.StaticData.Champions.GetAllAsync(latestVersion).Result.Champions.Values;
            
            if(champ.Count != _FieldBiz.ChampionCnt())
            {
                foreach(var p in champ)
                {
                    Champion champion = new Champion();
                    champion.id = p.Id;
                    champion.name = p.Name;
                    champion.imageSrc = p.Image.Full;
                    champion.skins = p.Skins.Count;
                    champion.passive = p.Passive.Description;
                    champion.title = p.Title;
                    _FieldBiz.InsertChampion(champion);
                }               
                result = 1;
            }
            else
            {
                result = -1; 
            }
           
            return result;
        }

       
    }
}
