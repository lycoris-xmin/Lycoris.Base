using Lycoris.Base.Extensions;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Base.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class FileChunkHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FileChunk CreateFileChunk(Action<FileChunk> configure)
        {
            var chunk = new FileChunk(Guid.NewGuid().ToString("N"));
            configure.Invoke(chunk);
            return chunk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FileChunk CreateFileChunk(IFormFile file, Action<FileChunk> configure)
        {
            var chunk = new FileChunk(Guid.NewGuid().ToString("N"));
            configure.Invoke(chunk);
            chunk.File = file;
            return chunk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkPath"></param>
        /// <returns></returns>
        public static FileChunkSort[] GetAllChunkFile(FileChunk chunk, string chunkPath)
        {
            var fileList = Directory.GetFiles(chunkPath, $"{chunk.FileName}{FileChunk.PART_NUMBER}*");
            if (!fileList.HasValue())
                return Array.Empty<FileChunkSort>();

            var sortArray = new FileChunkSort[fileList.Length];

            for (int i = 0; i < fileList.Length; i++)
            {
                var filePath = fileList[i];

                var partNumber = filePath[(filePath.IndexOf(FileChunk.PART_NUMBER) + FileChunk.PART_NUMBER.Length)..];

                var number = partNumber.ToTryInt();
                if (!number.HasValue || number.Value <= 0)
                    continue;

                sortArray[i] = new FileChunkSort
                {
                    FileName = Path.GetFileName(filePath),
                    ChunkPath = chunkPath,
                    PartNumber = number!.Value
                };
            }

            // 按照分片号码排序
            return sortArray.OrderBy(s => s.PartNumber).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task MergeFileChunks(FileChunkSort[] chunks, string path, string fileName)
        {
            var fullPath = Path.Combine(path, fileName.TrimStart('/').TrimStart('\\'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            int failedCount = 0;
            Exception? exception = null;
            using var fileStream = new FileStream(fullPath, FileMode.Create);

            foreach (var chunk in chunks)
            {
                failedCount = 0;
                do
                {
                    try
                    {
                        using var fileChunk = new FileStream(Path.Combine(chunk.ChunkPath, chunk.FileName), FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                        await fileChunk.CopyToAsync(fileStream);
                        failedCount = 0;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        failedCount++;
                        Thread.Sleep(10);
                    }
                } while (failedCount > 0 && failedCount < 100);

                if (failedCount >= 100)
                    throw new Exception("merge chunk files failed", exception);
            }

            chunks.ForEach(x => File.Delete(x.FileName));
        }
    }

    /// <summary>
    /// 文件批量上传的URL参数模型
    /// </summary>
    public class FileChunk
    {
        internal const string PART_NUMBER = ".partNumber";

        /// <summary>
        /// 
        /// </summary>
        public FileChunk()
        {
            TempId = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TempFile"></param>
        internal FileChunk(string TempFile)
        {
            TempId = TempFile;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TempId { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 当前分片
        /// </summary>
        public int PartNumber { get; set; }

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 分片总数
        /// </summary>
        public int Chunks { get; set; }

        /// <summary>
        /// 文件读取起始位置
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 文件读取结束位置
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal IFormFile? File { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public FileChunk Save(string directory)
        {
            if (File == null)
                throw new Exception("");

            if (FileName.IsNullOrEmpty())
                FileName = File!.FileName;
            using var ms = new MemoryStream();
            File.CopyTo(ms);
            ms.ToArray().SaveAs(Path.Combine(directory, TempId, GetChunkFileName(FileName, PartNumber)));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public FileChunk Save(byte[] bytes, string directory)
        {
            bytes.SaveAs(Path.Combine(directory, TempId, GetChunkFileName(FileName, PartNumber)));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public async Task<FileChunk> SaveAsync(byte[] bytes, string directory)
        {
            await bytes.SaveAsAsync(Path.Combine(directory, TempId, GetChunkFileName(FileName, PartNumber)));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        internal static string GetChunkFileName(string fileName, int partNumber) => $"{fileName}{PART_NUMBER}-{partNumber}";
    }

    /// <summary>
    /// 
    /// </summary>
    public class FileChunkSort
    {
        /// <summary>
        /// 带有序列号的文件名
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 存放分片的文件夹
        /// </summary>
        public string ChunkPath { get; set; } = string.Empty;

        /// <summary>
        ///PartNumber 
        /// </summary>
        public int PartNumber { get; set; }
    }
}
