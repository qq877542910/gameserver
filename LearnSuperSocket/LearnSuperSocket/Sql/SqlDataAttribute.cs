using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocket.Sql
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SqlDataAttribute : System.Attribute
    {
        public enum AttributeType
        {
            Request,
            NotMapper,
            Key,
        }

        private AttributeType _type;

        public SqlDataAttribute(AttributeType type)
        {
            _type = type;
        }

        public AttributeType Type
        {
            get
            {
                return _type;
            }
        }
    }
}
