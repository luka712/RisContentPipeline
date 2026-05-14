using System.Net;

namespace RisContentPipeline.GUI.Controls;

public class LocalWebServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly string _rootPath;
    private readonly int _port;
    private readonly CancellationTokenSource _cts = new();

    public int Port => _port;

    public LocalWebServer(string rootPath, int port = 8080)
    {
        _rootPath = rootPath;
        _port = port;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/");
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine($"🌐 Local server running at http://localhost:{_port}");

        _ = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = HandleRequestAsync(context);
                }
                catch { }
            }
        });
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            string path = context.Request.Url?.AbsolutePath.TrimStart('/') ?? "index.html";
            if (string.IsNullOrEmpty(path) || path == "/") path = "index.html";

            string fullPath = Path.Combine(_rootPath, path.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                string mimeType = GetMimeType(fullPath);
                context.Response.ContentType = mimeType;

                using var fs = File.OpenRead(fullPath);
                await fs.CopyToAsync(context.Response.OutputStream);
            }
            else
            {
                context.Response.StatusCode = 404;
                await using var writer = new StreamWriter(context.Response.OutputStream);
                await writer.WriteAsync("<h1>404 - File Not Found</h1>");
            }
        }
        catch
        {
            context.Response.StatusCode = 500;
        }
        finally
        {
            context.Response.Close();
        }
    }

    private static string GetMimeType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".html" => "text/html",
            ".css"  => "text/css",
            ".js"   => "application/javascript",
            ".png"  => "image/png",
            ".jpg"  => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif"  => "image/gif",
            ".svg"  => "image/svg+xml",
            ".json" => "application/json",
            _ => "application/octet-stream"
        };
    }

    public void Dispose()
    {
        _cts.Cancel();
        _listener.Stop();
        _listener.Close();
    }
}