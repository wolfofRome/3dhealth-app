using System;
using System.IO;

namespace Assets.Scripts.Common.Util
{
    public static class FileIO
    {
        /// <summary>
        /// ディレクトリの削除
        /// </summary>
        /// <param name="targetDirectoryPath"></param>
        public static void DeleteDirectory(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            // ディレクトリ以外の全ファイルを削除.
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            // ディレクトリ内のディレクトリも再帰的に削除.
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                DeleteDirectory(directoryPath);
            }

            // 空になったらディレクトリ自身も削除.
            Directory.Delete(targetDirectoryPath, false);
        }


        public static byte[] ReadBytes(string path)
        {
            byte[] data = null;

            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryReader bin = new BinaryReader(fileStream);
                data = bin.ReadBytes((int)bin.BaseStream.Length);
                bin.Close();
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.Message);
            }

            return data;
        } 
    }
}
