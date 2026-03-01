using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogSidecar.Services
{

public record LogEntry(
    DateTime  Timestamp,
    string    Level,
    string    Service,
    string    Message,
    object?   Data
);

    public class LogReader
    {
        private readonly string _logPath;

    public LogReader(IConfiguration config)
    {
        var path = config["SharedLogPath"]
                   ?? Path.Combine("..", "shared", "app.log");

        _logPath = Path.GetFullPath(path);
        Console.WriteLine($"[LogSidecar] İzlenen dosya → {_logPath}");
    }


      public List<LogEntry> ReadAll()
    {
        if (!File.Exists(_logPath)) return [];

        var lines = ReadLinesShared(_logPath);
        return ParseEntries(lines);
    }

       private static IEnumerable<string> ReadLinesShared(string path)
    {
        using var fs     = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs);
        var lines = new List<string>();
        while (reader.ReadLine() is { } line)
            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line);
        return lines;
    }
private static List<LogEntry> ParseEntries(IEnumerable<string> lines)
    {
        var entries = new List<LogEntry>();
        foreach (var line in lines)
        {
            try
            {
                var entry = JsonSerializer.Deserialize<LogEntry>(line,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (entry is not null) entries.Add(entry);
            }
            catch { }
        }
        return entries;
    }
    }
}