namespace OrderProcessingSystem.Contracts.Interfaces;

/// <summary>
/// Interface for providing SQL content from various sources (embedded files, database, etc.)
/// This is a contract that can be implemented by different providers in Infrastructure layer
/// </summary>
public interface ISqlProvider
{
    /// <summary>
    /// Gets SQL content by name/key
    /// </summary>
    /// <param name="name">The name/key of the SQL content to retrieve</param>
    /// <returns>The SQL content as a string</returns>
    string GetSql(string name);
}
