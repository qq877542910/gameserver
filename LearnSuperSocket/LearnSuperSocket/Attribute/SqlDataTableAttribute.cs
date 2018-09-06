using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class)]
public class SqlDataTableAttribute : System.Attribute
{
    private string _tableName;
    public SqlDataTableAttribute(string table)
    {
        _tableName = table;
    }

    public string tableName
    {
        get { return _tableName; }
    }
}