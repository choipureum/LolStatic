using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiotInfo.com.Util
{
    public class FieldInfo
    {
        public string rotation { get; set; }
        public int MaxNewPlayerLevel { get; set; }
        public List<int> rotationChamp { set; get; }
        public List<Champion> champions { get; set; }
    }
    public class ChampionMastery
    {
        public long id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int points { get; set; }
    }
    public class Tier
    {
        public string tier { get; set; }
        public string rank { get; set; }
        public int leaguePoints { get; set; }
        public string summonerId { get; set; }
        public string summonerName { get; set; }
        public string queueType { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
    }
    public class Champion
    {
        public long id { get; set; }
        public string name { get; set; }
        public string imageSrc { get; set; }
        public int skins { get; set; }
        public string passive { get; set; }
        public string title { get; set; }
    }
    
}