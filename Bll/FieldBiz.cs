using RiotInfo.com.Dal;
using RiotInfo.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiotInfo.com.Bll
{
    public class FieldBiz
    {
        private readonly FieldDal _dal = new FieldDal();
        public Champion RetrieveChampion(int id)
        {
            return _dal.RetrieveChampion(id);
        }

        public void InsertChampion(Champion champion)
        {
            _dal.InsertChampion(champion);
        }
        public int ChampionCnt()
        {
           return _dal.ChampionCnt();
        }
    }
}