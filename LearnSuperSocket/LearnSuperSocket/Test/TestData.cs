using LearnSuperSocket.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocket.Test
{
    public class TestData //: IKeyID
    {
        [SqlData(SqlDataAttribute.AttributeType.Key)]
        public int ID { get; set; }

        [SqlData(SqlDataAttribute.AttributeType.Request)]
        public string user { get; set; }

        [SqlData(SqlDataAttribute.AttributeType.Request)]
        public int score { get; set; }

        [SqlData(SqlDataAttribute.AttributeType.Request)]
        public string uid { get; set; }

        [SqlData(SqlDataAttribute.AttributeType.Request)]
        public string datas { get; set; }

        public int[] dataArr;
            
    }
}
