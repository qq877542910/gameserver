using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

namespace LearnSuperSocket.Sql
{
    public class MySqlHelper
    {
        private static MySqlHelper instance;

        public static MySqlHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MySqlHelper();
                }
                return instance;
            }
        }

        private string path = System.Environment.CurrentDirectory + @"\SqliteData\";
        private string dbName = "data.db";

        private SQLiteConnection cn;
        private SQLiteCommand cmd;

        public MySqlHelper()
        {
            Console.WriteLine("sql path : " + path + dbName);

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            if (!System.IO.File.Exists(path + dbName))
            {
                System.IO.File.Create(path + dbName);
            }

            cn = new SQLiteConnection("Data Source=" + path + dbName);
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
            }
            cmd = new SQLiteCommand();
            cmd.Connection = cn;
        }

        ~MySqlHelper()
        {
            if (cn != null && cn.State == System.Data.ConnectionState.Open)
            {
                cn.Close();
            }
        }

        public void Action()
        {

            CreateTable("player", new string[] { "user", "passward", "uid", "name", "sex", "age" }, new string[] { "char(40)", "char(40)", "char(64)", "char(40)", "char(10)", "char(10)" });
            InsertData("player", new string[] { "test01", "123456", "00001", "test", "women", "18" });

            //DeleteData("player", new string[] { "name", "sex" }, new string[] { "test", "women" }, new string[] { "and" });

            DataTable dt = SelectData("player", "user", "test01");

            DataTable dt2 = SelectData("player", null, null, null);

            TestData td = new LearnSuperSocket.TestData();
            td.uid = "0005";
            td.user = "te1";
            td.score = 500;
            td.datas = new int[] { 3, 8, 25, 194 };

            CreateTable(td);
            InsertData(td);
        }

        public void CreateTable(string tableName, string[] colNames, string[] colTypes)
        {
            if (colNames.Length != colTypes.Length)
            {
                throw new SQLiteException("sqlHelper createTable has error : colNames.length != colTypes.length");
            }

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName;

            string body = "";

            for (int i = 0; i < colNames.Length; i++)
            {
                body += string.Format("{0} {1} {2} ", colNames[i], colTypes[i], i < colNames.Length - 1 ? "," : "");
            }
            cmd.CommandText += "(" + body + ")";

            Console.WriteLine("createTable : " + cmd.CommandText);

            cmd.ExecuteNonQuery();
        }

        public void InsertData(string tableName, string[] colValues)
        {
            string str = string.Format("INSERT INTO {0} VALUES ", tableName);
            string body = "";

            for (int i = 0; i < colValues.Length; i++)
            {
                body += string.Format("'{0}' {1} ", colValues[i], i < colValues.Length - 1 ? "," : "");
            }
            str += "(" + body + ")";

            Console.WriteLine("sq insertData cmd : " + str);


            cmd.CommandText = str;
            cmd.ExecuteNonQuery();
        }

        public void UpdateData(string tableName, string[] colNames, string[] colValues, string[] whereNames, string[] whereValues)
        {
            string cmdStr = string.Format("UPDATE {0} SET", tableName);

            for (int i = 0; i < colNames.Length; i++)
            {
                cmdStr += string.Format(" {0}={1} ", colNames[i], colValues[i]);
            }

            if (whereNames != null && whereValues != null)
            {
                if (whereNames.Length != whereValues.Length)
                {
                    Console.WriteLine("sql update data have error : whereNames.length != whereValues.length!");
                    return;
                }

                cmdStr += "WHERE";
                for (int i = 0; i < whereNames.Length; i++)
                {
                    cmdStr += string.Format(" {0}={1} ", whereNames[i], whereValues[i]);
                }
            }

            Console.WriteLine("sql update cmd : " + cmdStr);

            cmd.CommandText = cmdStr;
            cmd.ExecuteNonQuery();
        }

        public void DeleteData(string tableName, string[] whereNames, string[] whereValues, string[] whereOperators)
        {
            string cmdStr = string.Format("DELETE FROM {0} ", tableName);

            if (whereNames != null && whereValues != null)
            {
                if (whereNames.Length != whereValues.Length)
                {
                    Console.WriteLine("sql DeleteData have error : whereNames.length != whereValues.length!");
                    return;
                }
                if (whereNames.Length > 1 && (whereOperators == null || whereOperators.Length != whereNames.Length - 1))
                {
                    Console.WriteLine("sql DeleteData have error : whereOperators.length != whereNames.Length -1");
                    return;
                }

                cmdStr += "WHERE";
                for (int i = 0; i < whereNames.Length; i++)
                {
                    cmdStr += string.Format(" {0}='{1}' {2}", whereNames[i], whereValues[i], i < whereNames.Length - 1 ? whereOperators[i] : "");
                }
            }

            Console.WriteLine("sql deleteData cmd : " + cmdStr);
            cmd.CommandText = cmdStr;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereNames"></param>
        /// <param name="whereValues"></param>
        /// <param name="whereOperators">where的操作符 : < , <= , > , >= , != , = </param>
        /// <param name="linkOperators">两个where表达式的连接符 and or</param>
        /// <returns></returns>
        public DataTable SelectData(string tableName, string colName, string colValue, string operators = "=")
        {
            string cmdStr = string.Format("SELECT * FROM {0} ", tableName);

            if (!string.IsNullOrEmpty(colName) && !string.IsNullOrEmpty(colValue) && !string.IsNullOrEmpty(operators))
            {
                cmdStr += string.Format("WHERE {0}{1}'{2}'", colName, operators, colValue);
            }

            Console.WriteLine("sql SelectData cmd : " + cmdStr);

            cmd.CommandText = cmdStr;

            using (SQLiteDataAdapter ap = new SQLiteDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                ap.Fill(ds);

                return ds.Tables[0];
            }
        }



        private string ObjectToJson(object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        private object JsonToObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        private void DoSqlCmd(string SqlStr, SQLiteParameter[] parameters)
        {
            Console.WriteLine("DoSqlCmd : " + SqlStr);
            cmd.CommandText = SqlStr;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            cmd.ExecuteNonQuery();
        }

        private SqlDataTableAttribute GetSqlTable<T>(T obj) where T : class
        {
            var tables = obj.GetType().GetCustomAttributes(typeof(SqlDataTableAttribute), true);
            if (tables != null && tables.Length > 0)
            {
                return (SqlDataTableAttribute)tables[0];
            }
            return null;
        }

        private Dictionary<FieldInfo, SqlDataAttribute> GetSqlField<T>(T obj) where T : class
        {
            Dictionary<FieldInfo, SqlDataAttribute> dict = new Dictionary<FieldInfo, SqlDataAttribute>();

            foreach (var val in obj.GetType().GetFields())
            {
                var datas = val.GetCustomAttributes(typeof(SqlDataAttribute), true);
                if (datas != null && datas.Length > 0)
                {
                    dict.Add(val, (SqlDataAttribute)datas[0]);
                }
            }

            return dict;
        }


        public void CreateTable<T>(T obj) where T : class
        {
            string tableName = GetSqlTable(obj).tableName;
            Dictionary<FieldInfo, SqlDataAttribute> filedDict = GetSqlField(obj);

            string cmdStr = "CREATE TABLE IF NOT EXISTS " + tableName;

            string body = "";

            int index = 0;
            foreach (var val in filedDict)
            {
                body += string.Format(" {0} {1} ", val.Value.ColName, val.Value.ColType);
                body += index < filedDict.Count - 1 ? ", " : " ";
                index++;
            }
            cmdStr += "(" + body + ")";

            DoSqlCmd(cmdStr,null);
            Console.WriteLine("create table : " + tableName);
        }

        public void InsertData<T>(T obj) where T : class
        {
            string tableName = GetSqlTable(obj).tableName;
            Dictionary<FieldInfo, SqlDataAttribute> dict = GetSqlField(obj);

            string cmdStr = string.Format("INSERT INTO {0} VALUES ", tableName);

            string body = "";

            SQLiteParameter[] parameters = new SQLiteParameter[dict.Count];

            int index = 0;
            foreach (var val in dict)
            {
                body += "@" + val.Key.Name + (index < dict.Count - 1 ? "," : "");
                parameters[index++] = new SQLiteParameter("@" + val.Key.Name, ObjectToJson(val.Key.GetValue(obj)));
            }

            cmdStr += "(" + body + ")";
            DoSqlCmd(cmdStr, parameters);
        }


        public void DeleteData<T>( string parameter) where T :class
        { }

        public T SelectData<T>(string parameter) where T: class
        {
            return null;
        }

        public List<T> SelectFullTable<T>() where T:class
        {
            return null;
        }
    }
}
