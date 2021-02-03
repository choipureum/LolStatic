using PR_LIB;
using RiotInfo.com.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RiotInfo.com.Dal
{
    /// <summary>
    /// champion select
    /// </summary>
    public class FieldDal
    {
        public Champion RetrieveChampion(int id)
        {
            Champion champ = new Champion();

            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "Champion@SelectChampionRota",
                CommandType = CommandType.StoredProcedure
            };
            sqlCmd.Parameters.Add("@id", SqlDbType.Int).Value = id;


            SqlDataReader reader = SQLHelper.ExecuteReader(sqlCmd);

            try
            {
                if (reader != null)
                {
                    champ.id = (long)reader["id"];
                    champ.name = reader["name"].ToString();
                    champ.imageSrc = reader["Imagesrc"].ToString();
                    champ.skins = (int)reader["skins"];
                    champ.passive = reader["pasive"].ToString();
                    champ.title = reader["title"].ToString();

                }
            }
            catch (Exception) { }
            finally
            {
                reader.Close();
            }

            return champ;
        }
        /// <summary>
        /// 챔피언 정보 입력
        /// </summary>
        /// <param name="champ"></param>
        public void InsertChampion(Champion champ)
        {
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "Champion@InsertInfo",
                CommandType = CommandType.StoredProcedure
            };
            sqlCmd.Parameters.Add("@id", SqlDbType.Int).Value = champ.id;
            sqlCmd.Parameters.Add("@name", SqlDbType.VarChar).Value = champ.name;
            sqlCmd.Parameters.Add("@Imagesrc", SqlDbType.VarChar).Value = champ.imageSrc;
            sqlCmd.Parameters.Add("@skins", SqlDbType.Int).Value = champ.skins;
            sqlCmd.Parameters.Add("@passive", SqlDbType.VarChar).Value = champ.passive;
            sqlCmd.Parameters.Add("@title", SqlDbType.VarChar).Value = champ.title;

            SQLHelper.ExecuteNonQuery(sqlCmd);
        }
        /// <summary>
        /// 챔피언 개수
        /// </summary>
        /// <returns></returns>
        public int ChampionCnt()
        {
            int result = 0;
            SqlCommand sqlCmd = new SqlCommand
            {
                CommandText = "Champion@TotalCnt",
                CommandType = CommandType.StoredProcedure
            };

            SqlDataReader reader = SQLHelper.ExecuteReader(sqlCmd);
            try
            {
                if (reader != null)
                {
                    result = (int)reader["totalCnt"];
                }
            }
            catch (Exception) { }
            finally
            {
                reader.Close();
            }
            return result;

        }
    }  
}

 

