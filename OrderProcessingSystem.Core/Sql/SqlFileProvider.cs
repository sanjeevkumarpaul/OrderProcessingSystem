using System.Collections.Concurrent;
using System.Reflection;

namespace OrderProcessingSystem.Core.Sql;

public class SqlFileProvider : ISqlProvider
{
    private readonly ConcurrentDictionary<string, string> _cache = new();
    private readonly Assembly _assembly;
    private readonly string _resourceRoot;

    public SqlFileProvider()
    {
        _assembly = Assembly.GetExecutingAssembly();
        // resources are embedded with the path: {DefaultNamespace}.Sql.<path>.sql
        _resourceRoot = _assembly.GetName().Name + ".Sql.";
    }

    public string GetSql(string name)
    {
        return _cache.GetOrAdd(name, key =>
        {
            var resourceName = _resourceRoot + key.Replace('.', '_').Replace('/', '_');
            // Our files were included as EmbeddedResource under Sql\sub\file.sql; manifest names use '.' separators
            // Try a few candidate patterns
            var candidates = new[] {
                _resourceRoot + key.Replace('.', '_') + ".sql",
                _resourceRoot + key.Replace('.', System.IO.Path.DirectorySeparatorChar) + ".sql",
                _resourceRoot + key.Replace('.', '.') + ".sql",
                _resourceRoot + key + ".sql"
            };

            foreach (var candidate in candidates)
            {
                var stream = _assembly.GetManifestResourceStream(candidate);
                if (stream != null)
                {
                    using var reader = new System.IO.StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }

            // fallback: try to locate any manifest resource that ends with the key
            var all = _assembly.GetManifestResourceNames();
            var match = all.FirstOrDefault(n => n.EndsWith(key + ".sql", StringComparison.OrdinalIgnoreCase) || n.EndsWith(key.Replace('.', '_') + ".sql", StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                using var s = _assembly.GetManifestResourceStream(match)!;
                using var r = new System.IO.StreamReader(s);
                return r.ReadToEnd();
            }

            throw new System.IO.FileNotFoundException($"Embedded SQL resource for key '{key}' not found. Available: {string.Join(',', all)}");
        });
    }
}
