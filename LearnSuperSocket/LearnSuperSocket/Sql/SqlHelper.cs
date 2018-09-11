using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using Dapper;

namespace LearnSuperSocket.Sql
{
    //sql数据类应该实现接口IKeyID
    //默认数据库表table名称为: 数据实体类class名全小写 +'s' ,如 ： Student.cs <--数据<=>表--> students

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

        private string fullPath
        {
            get { return path + dbName; }
        }

        private IDbConnection cn;

        public MySqlHelper()
        {
            Console.WriteLine("sql path : " + fullPath);

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            if (!System.IO.File.Exists(fullPath))
            {
                System.IO.File.Create(fullPath);
            }

            cn = new SQLiteConnection("Data Source=" + fullPath);
        }

        ~MySqlHelper()
        {
            if (cn != null)
            {
                cn.Dispose();
            }
        }

        private string[] GetSqlParamets<T>(T instance)
        {
            List<string> paramets = new List<string>();

            foreach (var val in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                SqlDataAttribute sdab = val.GetCustomAttribute<SqlDataAttribute>();
                if (CanAddParamets(sdab))
                {
                    paramets.Add(val.Name);
                }
            }

            return paramets.ToArray();
        }

        private bool CanAddParamets(SqlDataAttribute sdab)
        {
            if (sdab == null)
            {
                return false;
            }

            if (sdab.Type == SqlDataAttribute.AttributeType.Key || sdab.Type == SqlDataAttribute.AttributeType.Request)
            {
                return true;
            }

            return false;
        }

        private string GetTableName<T>()
        {
            return typeof(T).Name.ToLower() + "s";
        }

        public void ExecuteInsert<T>(T instance)
        {
            string tableName = GetTableName<T>();

            string[] paramets = GetSqlParamets(instance);

            string filed1 = string.Join(",", paramets);
            string filed2 = string.Join(",", paramets.Select(filed => "@" + filed));

            string cmdStr = string.Format("insert into {0}({1})values({2})", tableName, filed1, filed2);

            cn.Execute(cmdStr, instance);
        }

        public void ExecuteDelete<T>(int paramID)
        {
            string tableName = GetTableName<T>();
            string sql = "delete from " + tableName + " where ID = @ParamID";
            cn.Execute(sql, new { ParamID = paramID });
        }

        public T ExecuteSelectByID<T>(int paramID)
        {
            string tableName = GetTableName<T>();
            return cn.QuerySingleOrDefault<T>("SELECT * FROM " + tableName + " where ID = @ParamID", new { ParamID = paramID });
        }

        public List<T> ExecuteSelectFullTable<T>() where T : class
        {
            string sql = "SELECT * FROM " + GetTableName<T>();
            return cn.Query<T>(sql).ToList();
        }

        public void ExecuteUpdate<T>(object instance, params string[] fields)
        {
            string tableName = GetTableName<T>();

            if (fields.Length == 0)
            {
                fields = GetSqlParamets(instance);
            }

            var fieldsSql = String.Join(",", fields.Select(field => field + " = @" + field));

            var sql = String.Format("update {0} set {1} where ID = @ID", tableName, fieldsSql);

            cn.Execute(sql, instance);
        }

        public string ObjectToJson(object instance)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(instance);
        }

        public T JsonToObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
