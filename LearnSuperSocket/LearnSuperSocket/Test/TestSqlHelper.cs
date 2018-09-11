using LearnSuperSocket.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocket.Test
{
    public class TestSqlHelper
    {

        public void Test()
        {
            testDelete();
        }

        private void TestAdd()
        {
            TestData td = new TestData();
            td.ID = 50;
            td.user = "test01";
            td.uid = "00015";
            td.score = 50;
            td.dataArr = new int[] { 1, 3, 5, 67, 74 };
            td.datas = MySqlHelper.Instance.ObjectToJson(td.dataArr);

            MySqlHelper.Instance.ExecuteInsert(td);

            Console.WriteLine("test sql helper insert");
        }

        private void TestSelect()
        {
            TestData td = MySqlHelper.Instance.ExecuteSelectByID<TestData>(50);
            Console.WriteLine("td : " + td.score +" , "+ td.datas);
        }

        private void TestSelectTables()
        {
            List<TestData> tds = MySqlHelper.Instance.ExecuteSelectFullTable<TestData>();

            foreach (var val in tds)
            {
                Console.WriteLine(val.ID + " | " + val.user + " | " + val.datas);
            }
        }

        private void testDelete()
        {
            MySqlHelper.Instance.ExecuteDelete<TestData>(5);
        }
    }

}
