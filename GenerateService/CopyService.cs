using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenerateService
{
    public class CopyService
    {
        public string Message { get; set; }

        public bool Copy()
        {
            var files = GetControllers();
            CopyIservice(files);
            return true;
        }

        private bool CopyIservice(FileInfo[] fileInfos)
        {
            if (!fileInfos.Any())
            {
                return true;
            }

            return true;
        }

        private FileInfo[] GetControllers()
        {
            var directory = @"D:\DaiShuProgram\daishu.nationalsale\DaiShu.NationalSale.Controllers";
            if (!Directory.Exists(directory))
            {
                Message = "无效的文件路径";
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            var files = directoryInfo.GetFiles("*Controller.cs").Where(u => !u.Name.Contains("BaseController")).ToArray();
            return files;
        } 
    }
}