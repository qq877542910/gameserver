using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private string path = System.Environment.CurrentDirectory +@"\SqliteData\";
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
            SQLHelper
            CreateTable("player", new string[] { "uid", "user", "passward", "name", "sex", "age" }, new string[] { "char(50)","char(40)","char(40)","char(40)","char(10)","char(10)" });
            InsertData("player", new string[] { "000001", "test01", "123456", "test", "women", "18" });

            DeleteData("player", new string[] { "name", "sex" }, new string[] { "test", "women" }, new string[] { "and" });

            SQLiteDataReader sr = SeleteData("player", new string[] { "user", "passward" }, new string[] { "test01", "123456" }, new string[] { "and" });

            if (sr != null)
            {
                sr.Close();
            }
            SQLiteDataReader sr2 = SeleteData("player",null,null,null);
            sr2.Read();
        }

        public void CreateTable(string tableName, string[] colNames, string[] colTypes)
        {
            if (colNames.Length != colTypes.Length)
            {
                throw new SQLiteException("sqlHelper createTable has error : colNames.length != colTypes.length");
            }

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName;

            cmd.CommandText += "(";

            for (int i = 0; i < colNames.Length; i++)
            {
                cmd.CommandText += colNames[i] + " " + colTypes[i];
                if (i < colNames.Length - 1)
                {
                    cmd.CommandText += ",";
                }
            }
            cmd.CommandText += ")";

            Console.WriteLine("createTable : " + cmd.CommandText);
            cmd.ExecuteNonQuery();
        }

        public void InsertData(string tableName , string[] colValues)
        {
            string cmdStr = string.Format("INSERT INTO {0} VALUES",tableName);
            cmdStr += " (";
            for (int i = 0; i < colValues.Length; i++)
            {
                cmdStr += "'" + colValues[i] +"'";
                if (i < colValues.Length - 1)
                {
                    cmdStr += ",";
                }
            }
            cmdStr += ")";

            Console.WriteLine("sq insertData cmd : " + cmdStr);

            cmd.CommandText =cmdStr;
            cmd.ExecuteNonQuery();
        }

        public void UpdateData(string tableName , string[] colNames,string[] colValues , string[] whereNames, string[] whereValues)
        {
            string cmdStr = string.Format("UPDATE {0} SET", tableName);

            for (int i = 0; i < colNames.Length; i++)
            {
                cmdStr += string.Format(" {0}={1} ", colNames[i], colValues[i]);
            }

            if (whereNames != null && whereValues != null )
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

        public void DeleteData(string tableName,string[] whereNames, string[] whereValues , string[] whereOperators)
        {
            string cmdStr = string.Format("DELETE FROM {0} ", tableName);

            if (whereNames != null && whereValues != null)
            {
                if (whereNames.Length != whereValues.Length)
                {
                    Console.WriteLine("sql DeleteData have error : whereNames.length != whereValues.length!");
                    return;
                }
                if (whereNames.Length > 1 && (whereOperators ==null || whereOperators.Length != whereNames.Length - 1))
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

        public SQLiteDataReader SeleteData(string tableName, string[] whereNames, string[] whereValues , string[] whereOperators)
        {
            string cmdStr = string.Format("SELECT * FROM {0} ", tableName);

            if (whereNames != null && whereValues != null)
            {
                if (whereNames.Length != whereValues.Length)
                {
                    Console.WriteLine("sql SeleteData have error : whereNames.length != whereValues.length!");
                    return null;
                }
                if (whereNames.Length > 1 && (whereOperators == null || whereOperators.Length != whereNames.Length - 1))
                {
                    Console.WriteLine("sql SeleteData have error : whereOperators.length != whereNames.Length -1");
                    return null;
                }

                cmdStr += "WHERE";
                for (int i = 0; i < whereNames.Length; i++)
                {
                    cmdStr += string.Format(" {0}='{1}' {2}", whereNames[i], whereValues[i], i < whereNames.Length - 1 ? whereOperators[i] : "");
                }
            }

            Console.WriteLine("sql SelectData cmd : " + cmdStr);

            cmd.CommandText = cmdStr;
            SQLiteDataReader sr = cmd.ExecuteReader();
            return sr;
        }

      
    }
}
