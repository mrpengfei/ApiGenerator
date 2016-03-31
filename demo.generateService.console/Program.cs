using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenerateService;

namespace demo.generateService.console
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateService.GenerateService generateService = new GenerateService.GenerateService();
            var result = generateService.Generate();
            //CopyService copyService = new CopyService();
            //var result = copyService.Copy();
            if (result)
            {
                Console.WriteLine("生成完成，按任意键退出。。。");
            }
            else
            {
                //Console.WriteLine(copyService.Message);
                Console.WriteLine(generateService.Message);
            }
            Console.ReadKey();
        }
    }
}
