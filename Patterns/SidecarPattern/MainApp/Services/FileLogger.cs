using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainApp.Services
{

public record LogEntry(
    DateTime  Timestamp,
    string    Level,      // INFO / WARN / ERROR
    string    Service,    // "MainApp"
    string    Message,
    object?   Data = null
);


public class FileLogger
{
    private readonly string _logPath;
    private readonly Lock   _lock = new();

    public FileLogger(IConfiguration config)
    {
        var path = config["SharedLogPath"]
                   ?? Path.Combine("..", "shared", "app.log");

        _logPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);

        Console.WriteLine($"[FileLogger] Log dosyası → {_logPath}");
    }

    public void Info(string message, object? data = null)
        => Write("INFO", message, data);

    public void Warn(string message, object? data = null)
        => Write("WARN", message, data);

    public void Error(string message, object? data = null)
        => Write("ERROR", message, data);

    private void Write(string level, string message, object? data)
    {
        var entry = new LogEntry(DateTime.UtcNow, level, "MainApp", message, data);
        var line  = JsonSerializer.Serialize(entry);   // her satır = 1 JSON nesnesi

        lock (_lock)
        {
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }

        Console.WriteLine($"[{level}] {message}");
    }
}

}