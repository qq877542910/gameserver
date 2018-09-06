using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocket.Module.Player
{
    class PlayerVoData
    {
        public long uid;
        public string user;
        public string passward;
        public string name;
        public string sex;
        public string age;

        public static void CreateSqlTable()
        {
            Sql.MySqlHelper.Instance.CreateTable("PlayerVoData", new string[] {"uid", "user", "passward", "name", "sex", "age" }, new string[] { "char(50)", "char(40)", "char(40)", "char(40)", "char(40)", "char(40)" });
        }

        public static void DeleteSqlData(long uid)
        {
            Sql.MySqlHelper.Instance.DeleteData("PlayerData", new string[] { "uid" }, new string[] { uid.ToString() }, null);
        }

        public List<PlayerVoData> GetSqlAllData()
        {
            List<PlayerVoData> dataList = new List<Player.PlayerVoData>();

            return null;
        }
    }
}
