using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LearnSuperSocket
{
    [SqlDataTable("Test")]
   public class TestData
    {
        [SqlData("User","char(40)")]
        public string user;

        [SqlData("Score", "int")]
        public int score;

        [SqlData("Uid", "char(40)")]
        public string uid;

        [SqlData("Datas","char(40)")]
        public int[] datas;
    }
}
