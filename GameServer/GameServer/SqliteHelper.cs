using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GameServer
{
    class SqliteHelper
    {
        private string path = "E:\\SqliteData\\data.db";

        private SQLiteConnection cn;
        private SQLiteCommand cmd;

        public SqliteHelper()
        {
            cn = new SQLiteConnection("Data Source=" + path);
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
            }
            cmd = new SQLiteCommand();
            cmd.Connection = cn;
        }

        ~SqliteHelper()
        {
            if (cn != null && cn.State == System.Data.ConnectionState.Open)
            {
                cn.Close();
            }
        }

        public void Action()
        {
            CreateTable("t5", new string[] { "id","rank" }, new string[] { "varchar(4)", "int" });

            UpdateData();
        }

        public void CreateTable(string tableName , string[] colNames , string[] colTypes)
        {
            if (colNames.Length != colTypes.Length)
            {
                throw new SQLiteException("sqlHelper createTable has error : colNames.length != colTypes.length");
            }

            //cmd.CommandText = "CREATE TABLE IF NOT EXISTS t1(id varchar(4),score int)";

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

        public void InsertData()
        {
            cmd.CommandText = "INSERT INTO t1 VALUES(\"9999\",11)";
            cmd.ExecuteNonQuery();
        }

        public void UpdateData()
        {
            cmd.CommandText = "UPDATE t1 SET score=80 WHERE id = 1";
            cmd.ExecuteNonQuery();
        }

        public void DeleteData()
        {
            cmd.CommandText = "DELETE FROM t1 WHERE id =258";
            cmd.ExecuteNonQuery();
        }

        public void SeleteData()
        {
            cmd.CommandText = "SELECT * FROM t1 WHERE rowid=2";
            SQLiteDataReader sr = cmd.ExecuteReader();
            sr.Read();
            Console.WriteLine("data : " + sr.GetString(0)+" , " + sr.GetInt32(1).ToString());
        }
    }
}
