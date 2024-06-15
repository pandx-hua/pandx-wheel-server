namespace Sample.Application.Files.Dto;

public class DownloadRequest
{
    //下载上传到磁盘中的文件时需要传递的参数
    public string? FileName { get; set; }

    //下载从缓冲中保存的文件时需要传递的参数
    public string? Name { get; set; }

    //下载从缓冲中保存的文件时需要传递的参数
    public string? Token { get; set; }

    //下载数据库中保存的文件时需要传递的参数
    public Guid? ObjectId { get; set; }

    //下载数据库中保存的文件时需要传递的参数
    public string? ContentType { get; set; }
}