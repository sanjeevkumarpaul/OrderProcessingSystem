namespace OrderProcessingSystem.Core.Sql;

public interface ISqlProvider
{
    string GetSql(string name);
}
