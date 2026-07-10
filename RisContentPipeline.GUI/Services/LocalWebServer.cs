using System.Net;

namespace RisContentPipeline.GUI.Services;

/// <summary>
/// A local web server that serves static files for the KTX2 viewer.
/// </summary>
public class LocalWebServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly string _rootPath;
    private readonly int _port;
    private readonly CancellationTokenSource _cts = new();
    private Task? _serverTask;

    public int Port => _port;
    public bool IsListening => _listener.IsListening;

    public event Action<string>? OnLog;

    /// <summary>
    /// Creates a new local web server.
    /// </summary>
    /// <param name="rootPath">The root path to serve files from.</param>
    /// <param name="port">The port to listen on.</param>
    public LocalWebServer(string rootPath, int port)
    {
        _rootPath = rootPath;
        _port = port;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/");
    }

    /// <summary>
    /// Starts the local web server.
    /// </summary>
    public void Start()
    {
        if (_listener.IsListening) return;

        try
        {
            _listener.Start();
            OnLog?.Invoke($"🌐 Local server running at http://localhost:{_port}");

            _serverTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var context = await _listener.GetContextAsync();
                        _ = HandleRequestAsync(context);
                    }
                    catch (HttpListenerException) when (_cts.Token.IsCancellationRequested)
                    {
                        // Expected when stopping
                        break;
                    }
                    catch (Exception ex)
                    {
                        OnLog?.Invoke($"Server error: {ex.Message}");
                    }
                }
            }, _cts.Token);
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"Failed to start server: {ex.Message}");
        }
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
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");

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
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".mjs" => "application/javascript",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".json" => "application/json",
            ".wasm" => "application/wasm",
            ".ktx2" => "image/ktx2",
            _ => "application/octet-stream"
        };
    }

    public void Dispose()
    {
        _cts.Cancel();
        if (_listener.IsListening)
        {
            _listener.Stop();
            _listener.Close();
        }
        _cts.Dispose();
    }
}
