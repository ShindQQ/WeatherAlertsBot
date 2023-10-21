using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WeatherAlertsBot.HtmlConverter;

/// <summary>
/// Html Converter. Converts HTML string and URLs to image bytes
/// </summary>
public static class HtmlConverter
{
    /// <summary>
    ///     Tool file name
    /// </summary>
    private const string ToolFilename = "wkhtmltoimage";

    /// <summary>
    ///     Tool Directory
    /// </summary>
    private static readonly string Directory;

    /// <summary>
    ///  Tool`s path to file
    /// </summary>
    private static readonly string ToolFilepath;

    /// <summary>
    ///     Static constructor
    /// </summary>
    /// <exception cref="Exception">OSX is not implemented</exception>
    static HtmlConverter()
    {
        Directory = AppContext.BaseDirectory;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ToolFilepath = Path.Combine(Directory, ToolFilename + ".exe");

            if (File.Exists(ToolFilepath)) 
                return;

            var assembly = typeof(HtmlConverter).GetTypeInfo().Assembly;
            var type = typeof(HtmlConverter);
            var ns = type.Namespace;

            using var resourceStream = assembly.GetManifestResourceStream($"{ns}.{ToolFilename}.exe");
            using var fileStream = File.OpenWrite(ToolFilepath);
            resourceStream?.CopyTo(fileStream);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var process = Process.Start(new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = "/bin/",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = "/bin/bash",
                Arguments = "which wkhtmltoimage"
            });

            var answer = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();

            if (!string.IsNullOrEmpty(answer) && answer.Contains("wkhtmltoimage"))
            {
                ToolFilepath = "wkhtmltoimage";
            }
            else
            {
                throw new Exception("wkhtmltoimage does not appear to be installed on this linux system according to which command; go to https://wkhtmltopdf.org/downloads.html");
            }
        }
        else
        {
            throw new Exception("OSX Platform not implemented yet");
        }
    }

    /// <summary>
    ///     Converts HTML string to image
    /// </summary>
    /// <param name="html">HTML string</param>
    /// <param name="width">Output document width</param>
    /// <param name="format">Output image format</param>
    /// <param name="quality">Output image quality 1-100</param>
    /// <returns>Converted html string in byte array</returns>
    public static byte[] FromHtmlString(string html, int width = 1024, ImageFormat format = ImageFormat.Jpg, int quality = 100)
    {
        var filename = Path.Combine(Directory, $"{Guid.NewGuid()}.html");
        File.WriteAllText(filename, html);

        var bytes = FromUrl(filename, width, format, quality);
        File.Delete(filename);

        return bytes;
    }

    /// <summary>
    ///     Converts HTML page to image
    /// </summary>
    /// <param name="url">Valid http(s):// URL</param>
    /// <param name="width">Output document width</param>
    /// <param name="format">Output image format</param>
    /// <param name="quality">Output image quality 1-100</param>
    /// <returns>Converted url in byte array</returns>
    public static byte[] FromUrl(string url, int width = 1024, ImageFormat format = ImageFormat.Jpg, int quality = 100)
    {
        var imageFormat = format.ToString().ToLower();
        var filename = Path.Combine(Directory, $"{Guid.NewGuid()}.{imageFormat}");

        var args = $"--transparent --quality {quality} --width {width} -f {imageFormat} {url} \"{filename}\"";

        if (IsLocalPath(url))
        {
            args = $"--transparent --quality {quality} --width {width} -f {imageFormat} \"{url}\" \"{filename}\"";
        }

        var process = Process.Start(new ProcessStartInfo(ToolFilepath, args)
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            WorkingDirectory = Directory,
            RedirectStandardError = true
        });

        process!.ErrorDataReceived += Process_ErrorDataReceived;
        process?.WaitForExit();

        if (!File.Exists(filename)) 
            throw new Exception("Something went wrong. Please check input parameters");

        var bytes = File.ReadAllBytes(filename);
        File.Delete(filename);

        return bytes;

    }

    /// <summary>
    ///     Check is path local or url
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>True if it is path, false if url</returns>
    private static bool IsLocalPath(string path) =>
        !path.StartsWith("http") && new Uri(path).IsFile;

    private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e) =>
        throw new Exception(e.Data);
}
