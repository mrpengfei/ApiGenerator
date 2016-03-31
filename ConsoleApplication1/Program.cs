using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
             GenerateService.GenerateService generateService = new GenerateService.GenerateService();
            var result = generateService.Generate();
            if (result)
            {
                Console.WriteLine("生成完成，按任意键退出。。。");
            }
            else
            {
                Console.WriteLine(generateService.Message);
            }
            Console.ReadKey();
        }
    }
}
