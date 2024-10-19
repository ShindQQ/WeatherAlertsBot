using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WeatherAlertsBot.HtmlКонвертер;

/// <summary>
///     Html Converter. Converts HTML string and URLs to image bytes
/// </summary>
public static class HtmlКонвертер
{
    /// <summary>
    ///     Tool file name
    /// </summary>
    private const string НазваФайлуІнструмента = "wkhtmltoimage";

    /// <summary>
    ///     Tool Directory
    /// </summary>
    private static readonly string Діректорія;

    /// <summary>
    ///     Tool`s path to file
    /// </summary>
    private static readonly string ШляхІнструмента;

    /// <summary>
    ///     Static constructor
    /// </summary>
    /// <exception cref="Exception">OSX is not implemented</exception>
    static HtmlКонвертер()
    {
        Діректорія = AppContext.BaseDirectory;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ШляхІнструмента = Path.Combine(Діректорія, НазваФайлуІнструмента + ".exe");

            if (File.Exists(ШляхІнструмента))
                return;

            var збірка = typeof(HtmlКонвертер).GetTypeInfo().Assembly;
            var тип = typeof(HtmlКонвертер);
            var простірІмен = тип.Namespace;

            using var потікРесурсів = збірка.GetManifestResourceStream($"{простірІмен}.{НазваФайлуІнструмента}.exe");
            using var файловийПотік = File.OpenWrite(ШляхІнструмента);
            потікРесурсів?.CopyTo(файловийПотік);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var процес = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = "/bin/",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = "/bin/bash",
                Arguments = "which wkhtmltoimage"
            });

            var answer = процес?.StandardOutput.ReadToEnd();
            процес?.WaitForExit();

            if (!string.IsNullOrEmpty(answer) && answer.Contains("wkhtmltoimage"))
                ШляхІнструмента = "wkhtmltoimage";
            else
                throw new Exception(
                    "wkhtmltoimage does not appear to be installed on this linux system according to which command; go to https://wkhtmltopdf.org/downloads.html");
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
    /// <param name="ширина">Output document width</param>
    /// <param name="формат">Output image format</param>
    /// <param name="quality">Output image quality 1-100</param>
    /// <returns>Converted html string in byte array</returns>
    public static byte[] ІзHtmlРядка(string html, int ширина = 1024, ФорматЗображення формат = ФорматЗображення.Jpg,
        int quality = 100)
    {
        var назваФайлу = Path.Combine(Діректорія, $"{Guid.NewGuid()}.html");
        File.WriteAllText(назваФайлу, html);

        var байти = ІзUrl(назваФайлу, ширина, формат, quality);
        File.Delete(назваФайлу);

        return байти;
    }

    /// <summary>
    ///     Converts HTML page to image
    /// </summary>
    /// <param name="url">Valid http(s):// URL</param>
    /// <param name="ширина">Output document width</param>
    /// <param name="формат">Output image format</param>
    /// <param name="якість">Output image quality 1-100</param>
    /// <returns>Converted url in byte array</returns>
    private static byte[] ІзUrl(string url, int ширина = 1024, ФорматЗображення формат = ФорматЗображення.Jpg, int якість = 100)
    {
        var форматЗображення = формат.ToString().ToLower();
        var назваФайлу = Path.Combine(Діректорія, $"{Guid.NewGuid()}.{форматЗображення}");

        var арги = $"--transparent --quality {якість} --width {ширина} -f {форматЗображення} {url} \"{назваФайлу}\"";

        if (ЄЛокальнимШляхом(url))
            арги = $"--transparent --quality {якість} --width {ширина} -f {форматЗображення} \"{url}\" \"{назваФайлу}\"";

        var процес = Process.Start(new ProcessStartInfo(ШляхІнструмента, арги)
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            WorkingDirectory = Діректорія,
            RedirectStandardError = true
        });

        процес!.ErrorDataReceived += Процес_ДаніПомилкиОтримані;
        процес.WaitForExit();

        if (!File.Exists(назваФайлу))
            throw new Exception("Something went wrong. Please check input parameters");

        var байти = File.ReadAllBytes(назваФайлу);
        File.Delete(назваФайлу);

        return байти;
    }

    /// <summary>
    ///     Check is path local or url
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>True if it is path, false if url</returns>
    private static bool ЄЛокальнимШляхом(string path)
    {
        return !path.StartsWith("http") && new Uri(path).IsFile;
    }

    private static void Процес_ДаніПомилкиОтримані(object sender, DataReceivedEventArgs e)
    {
        throw new Exception(e.Data);
    }
}