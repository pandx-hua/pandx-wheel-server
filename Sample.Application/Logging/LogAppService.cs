using System.IO.Compression;
using pandx.Wheel.Folders;
using pandx.Wheel.Helpers;
using pandx.Wheel.MimeTypes;
using pandx.Wheel.Storage;
using Sample.Application.Logging.Dto;

namespace Sample.Application.Logging;

public class LogAppService : SampleAppServiceBase, ILogAppService
{
    private readonly ICachedFileManager _cachedFileManager;
    private readonly ICommonFolder _commonFolder;

    public LogAppService(ICommonFolder commonFolder, ICachedFileManager cachedFileManager)
    {
        _commonFolder = commonFolder;
        _cachedFileManager = cachedFileManager;
    }

    public async Task<CachedFile> DownloadLogsAsync()
    {
        var logFiles = GetAllLogFiles();
        var zipFile = new CachedFile("pandx.wheel.logs.zip", MimeTypes.ApplicationZip);
        using var zipStream = new MemoryStream();
        //此处不能使用using自动释放，否则生成的压缩文件无法正解压，报“文件末端错误”，
        var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);
        foreach (var logFile in logFiles)
        {
            var entry = zipArchive.CreateEntry(logFile.Name);
            await using var entryStream = entry.Open();
            await using (var fs = new FileStream(logFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite,
                             0x1000, FileOptions.SequentialScan))
            {
                await fs.CopyToAsync(entryStream);
            }

            await entryStream.FlushAsync();
        }

        //此处手动释放
        zipArchive.Dispose();
        await _cachedFileManager.SetFileAsync(zipFile.Token, zipFile.Name, zipStream.ToArray());
        return zipFile;
    }

    public Task<GetLatestLogsResponse> GetLatestLogsAsync()
    {
        var directory = new DirectoryInfo(_commonFolder.LogsFolder);
        if (!directory.Exists)
        {
            return Task.FromResult(new GetLatestLogsResponse());
        }

        var latestLogFile = GetAllLogFiles().MaxBy(f => f.LastWriteTime);
        if (latestLogFile is null)
        {
            return Task.FromResult(new GetLatestLogsResponse());
        }

        var lines = FileHelper.ReadLines(latestLogFile.FullName).Reverse().Take(1000).ToList();
        var usedLineCount = 0;
        var lineCount = 0;
        foreach (var line in lines)
        {
            if (line.StartsWith("VER") || line.StartsWith("DEB") || line.StartsWith("INF") || line.StartsWith("WAR") ||
                line.StartsWith("ERR") || line.StartsWith("FAT"))
            {
                usedLineCount++;
            }

            lineCount++;
            if (usedLineCount == 100)
            {
                break;
            }
        }

        return Task.FromResult(new GetLatestLogsResponse
        {
            LatestLogLines = lines.Take(lineCount).ToList()
        });
    }

    private List<FileInfo> GetAllLogFiles()
    {
        var directory = new DirectoryInfo(_commonFolder.LogsFolder);
        return directory.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
    }
}