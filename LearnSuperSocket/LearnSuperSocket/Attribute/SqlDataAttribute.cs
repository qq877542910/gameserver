using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Field)]
public class SqlDataAttribute : System.Attribute
{
    private string _colName;
    private string _colType;

    public SqlDataAttribute(string name, string type)
    {
        _colName = name;
        _colType = type;
    }

    public string ColName
    {
        get { return _colName; }
    }

    public string ColType
    {
        get { return _colType; }
    }
}

