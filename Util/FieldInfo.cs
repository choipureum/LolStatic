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
    }
    public class ChampionMastery
    {
        public long id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int points { get; set; }
    }
   
}