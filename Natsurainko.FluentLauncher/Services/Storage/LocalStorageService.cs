using System;
using System.IO;
using Windows.Storage;

namespace Natsurainko.FluentLauncher.Services.Storage;

internal class LocalStorageService
{
    /// <summary>
    /// 本地应用数据存储的完整路径
    /// </summary>
    public static string LocalFolderPath { get; }

    // 删除了 MsixPackageUtils.IsPackaged 的判断，因为再 Fluent Launcher 后期版本中没有再发行 Unpackaged 的程序包
    static LocalStorageService()
    {
        LocalFolderPath = ApplicationData.Current.LocalFolder.Path;
    }

    /// <summary>
    /// 确定给定路径下的文件是否存在于本地应用数据存储中
    /// </summary>
    /// <param name="path">本地应用数据存储中的文件路径</param>
    /// <returns>如果该路径指向一个现有的文件则返回 true</returns>
    public bool HasFile(string path)
        => File.Exists(Path.Combine(LocalFolderPath, path));

    /// <summary>
    ///确定给定路径下的文件夹是否存在于本地应用数据存储中
    /// </summary>
    /// <param name="path">本地应用数据存储中的文件夹路径</param>
    /// <returns>如果该路径指向一个现有的文件夹则返回 true</returns>
    public bool HasDirectory(string path)
    {
        string? directoryName = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(directoryName))
            throw new InvalidOperationException("E004");

        return Directory.Exists(Path.Combine(directoryName, path));
    }

    /// <summary>
    /// 获取本地应用数据存储中的文件 <br/>
    /// 这方法不能保证该文件存在
    /// </summary>
    /// <param name="path">本地应用数据存储中的文件路径</param>
    /// <returns>文件的 FileInfo 对象</returns>
    public FileInfo GetFile(string path)
    {
        string fullPath = Path.Combine(LocalFolderPath, path);

        return new FileInfo(fullPath);
    }

    /// <summary>
    /// 获取本地应用数据存储中的文件夹<br/>
    /// 如果不存在则创建目录
    /// </summary>
    /// <param name="path">本地应用数据存储中的文件夹路径</param>
    /// <returns>文件夹的 DirectoryInfo 对象</returns>
    public DirectoryInfo GetDirectory(string path)
    {
        string fullPath = Path.Combine(LocalFolderPath, path);

        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        return new DirectoryInfo(fullPath);
    }
}
