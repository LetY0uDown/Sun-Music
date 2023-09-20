﻿using Desktop_Client.Core.Tools.Attributes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

[BaseType]
public interface IFileManager : IService
{
    Task<Stream> DownloadStream(Guid fileID, string path);

    Task UploadFileAsync(string filePath, string path);
}