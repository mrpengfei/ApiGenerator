using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WebApiGenerator
{
    public class GenerateService
    {
        /// <summary>
        /// 设置或取得  生成消息
        /// </summary>
        /// 创建者：史鹏飞
        /// 创建日期：2016/2/25 17:20
        /// 修改者：
        /// 修改时间：
        /// ----------------------------------------------------------------------------------------
        public string Message { get; set; }

        public string BasePath { get; set; }

        //private readonly string modelDirectory = @"D:\DaiShuProgram\daishu.nationalsale\DaiShu.NationalSale.Model\Controller";

        //private readonly string sourceSuffix = "Request";

        //private readonly string destinationSuffix = "Parameter";

        //private readonly string sourceParameterName = "request";

        //private readonly string destinationParameterName = "parameter";

        //private readonly string nameSpace = "DaiShu.NationalSale";

        //private readonly string apiAddressPrefix = "daishu/nationalsale/api/";

        private readonly string modelDirectory = @"D:\DaiShuProgram\daishu.crm\DaiShu.Crm.Model\Controller";

        private readonly string sourceSuffix = "Request";

        private readonly string destinationSuffix = "Parameter";

        private readonly string sourceParameterName = "request";

        private readonly string destinationParameterName = "parameter";

        private readonly string nameSpace = "DaiShu.Crm.Admin";

        private readonly string apiAddressPrefix = "daishu/crm/api/";





        public GenerateService()
        {
        }

        public bool Generate()
        {
            var basePath = GenerateInit();

            Assembly assembly = GetAssembly();
            if (assembly == null)
            {
                return false;
            }

            var controllers = GetControllers(assembly);
            if (!controllers.Any())
            {
                return false;
            }

            foreach (var controller in controllers)
            {
                CreateIservice(controller);

                CreateService(controller);
            }
            CreateServiceConfig(controllers);

            CreateServiceModel();
            return true;
        }

        private string GenerateInit()
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GenerateFold");
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
            BasePath = basePath;
            return basePath;
        }

        private void CreateServiceModel()
        {
            var directory = modelDirectory;
            var dic = new DirectoryInfo(directory);
            var files = dic.GetFiles();
            foreach (var fileInfo in files)
            {
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GenerateFold");
                var path = string.Format("{0}/Model/{1}", basePath, fileInfo.Name.Replace(sourceSuffix, destinationSuffix));
                var text = fileInfo.OpenText().ReadToEnd();
                using (var reader = fileInfo.OpenText())
                {
                    text = reader.ReadToEnd();
                    text = text.Replace(string.Format("{0}.Model.Controller", GetApiNameSpace(nameSpace)), String.Format("{0}.Model.Services", nameSpace));
                    text = text.Replace(sourceSuffix, destinationSuffix);
                    CreateFile(path, text);
                }
            }
        }

        private bool CreateIservice(Type controller)
        {
            var serviceName = controller.Name.Replace("Controller", "Service");
            var iserviceName = "I" + serviceName;
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GenerateFold");
            var iservicePath = string.Format("{0}/IService/{1}.cs", basePath, iserviceName);

            var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"template\IService.txt");
            var text = File.ReadAllText(template);
            text = text.Replace("{namespace}", nameSpace);
            text = text.Replace("{serviceName}", iserviceName);
            text = text.Replace("{createtime}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));

            var methods = controller.GetMethods().Where(u => u.DeclaringType == controller && u.ReturnType.GenericTypeArguments.Length > 0);
            var methodsb = new StringBuilder();
            foreach (var methodInfo in methods)
            {
                methodsb.Append(GetMethodString(methodInfo));
                methodsb.AppendLine();
                methodsb.AppendLine();
            }
            text = text.Replace("{method}", methodsb.ToString());

            if (!CreateFile(iservicePath, text))
            {
                return false;
            }
            return true;
        }

        private bool CreateService(Type controller)
        {
            var serviceName = controller.Name.Replace("Controller", "Service");

            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GenerateFold");

            var servicePath = string.Format("{0}/Service/{1}.cs", basePath, serviceName);

            var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"template\Service.txt");
            var text = File.ReadAllText(template);
            text = text.Replace("{namespace}", nameSpace);
            text = text.Replace("{serviceName}", serviceName);
            text = text.Replace("{createtime}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
            text = text.Replace("{routePrefix}", GetRoutePrefix(controller));

            var methods = controller.GetMethods().Where(u => u.DeclaringType == controller && u.ReturnType.GenericTypeArguments.Length > 0);
            var methodsb = new StringBuilder();
            foreach (var methodInfo in methods)
            {
                methodsb.Append(GetServiceMethodString(methodInfo));
                methodsb.AppendLine();
                methodsb.AppendLine();
            }
            text = text.Replace("{method}", methodsb.ToString());


            if (!CreateFile(servicePath, text))
            {
                return false;
            }
            return true;
        }

        public string GetRoutePrefix(Type controller)
        {
            var routePrefix = controller.GetCustomAttribute<RoutePrefixAttribute>();
            return routePrefix.Prefix.Replace(apiAddressPrefix, "");
        }


        private string GetMethodString(MethodInfo methodInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetParameterDescription(methodInfo));
            sb.AppendFormat("            {0} {1}({2});", GetReturnParaName(methodInfo), methodInfo.Name, GetParameter(methodInfo));
            return sb.ToString();
        }

        private string GetServiceMethodString(MethodInfo methodInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetParameterDescription(methodInfo));
            sb.AppendFormat("        public {0} {1}({2})", GetReturnParaName(methodInfo), methodInfo.Name, GetParameter(methodInfo));
            sb.AppendLine("");
            sb.AppendLine("        {");
            sb.AppendLine(GetMethodBody(methodInfo));
            sb.AppendLine("        }");
            return sb.ToString();
        }

        private string GetMethodBody(MethodInfo methodInfo)
        {
            var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"template\PostMethod.txt");
            var text = File.ReadAllText(template);
            text = text.Replace("{postData}", string.Format("{0}", GetPostData(methodInfo)));
            text = text.Replace("{url}", GetUrl(methodInfo));
            text = text.Replace("{getResult}", GetResult(methodInfo));

            return text;
        }

        private string GetResult(MethodInfo methodInfo)
        {
            var httpGet = methodInfo.GetCustomAttribute<HttpGetAttribute>();
            var type = methodInfo.ReturnType;
            var parameterTypeName = GetParameterTypeName(type.GenericTypeArguments[0].Name);
            if (httpGet != null)
            {
                if (type.Name.Contains("ResponseList"))
                {
                    return "this.GetResultList<" + parameterTypeName + ">(url)";
                }
                return "this.GetResult<" + parameterTypeName + ">(url)";
            }
            return "this.Post<" + parameterTypeName + ">(url,postData)";
        }

        private string GetUrl(MethodInfo methodInfo)
        {
            var route = methodInfo.GetCustomAttribute<RouteAttribute>();
            if (route == null)
            {
                return "";
            }

            var controller = methodInfo.DeclaringType;
            var routePrefix = controller.GetCustomAttribute<RoutePrefixAttribute>();
            var parameters = methodInfo.GetParameters();
            string routeTem = route.Template;
            string urlPara = "";
            string urlParaV = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Name == sourceParameterName)
                {
                    continue;
                }
                urlParaV += "," + parameters[i].Name;
                var rep = "{" + parameters[i].Name + "}";
                if (routeTem.Contains(rep))
                {
                    routeTem = routeTem.Replace(rep, "{" + i + "}");
                }
                else
                {
                    if (urlPara != "")
                    {
                        urlPara += "&";
                    }
                    urlPara += parameters[i].Name + "={" + i + "}";
                }
            }
            var url = routeTem;
            if (!string.IsNullOrEmpty(urlPara))
            {
                url += "?" + urlPara;
            }
            return string.Format("string.Format(\"{0}\"{1})",
                url, urlParaV);
        }

        private string GetPostData(MethodInfo methodInfo)
        {
            var result = "var postData =";
            var httpGet = methodInfo.GetCustomAttribute<HttpGetAttribute>();
            if (httpGet != null)
            {
                return string.Empty;
            }
            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length == 1 && parameters[0].Name == sourceParameterName)
            {
                return result + destinationParameterName + ".Serialize();";
            }
            return result + "\"\";";
        }



        private string GetReturnParaName(MethodInfo method)
        {
            var type = method.ReturnType;
            if (type.Name.Contains("ResponseList"))
            {
                return "ServiceResultList<" + GetParameterTypeName(type.GenericTypeArguments[0].Name) + ">";
            }

            return "ServiceResult<" + GetParameterTypeName(type.GenericTypeArguments[0].Name) + ">";
        }

        private string GetParameter(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                var item = parameters[i];
                if (i != 0)
                {
                    sb.Append(",");
                }
                sb.AppendFormat("{0} {1}", GetParameterType(item),
                    item.Name.Replace(sourceParameterName, destinationParameterName));
                if (item.HasDefaultValue)
                {
                    sb.Append(" = ");
                    if (item.RawDefaultValue == "")
                    {
                        sb.Append("\"\"");
                    }
                    if (item.RawDefaultValue == null)
                    {
                        sb.Append("null");
                    }
                    sb.Append(item.RawDefaultValue);
                }
            }
            return sb.ToString();
        }

        private string GetParameterType(ParameterInfo parameter)
        {
            var parameterTypeName = parameter.ParameterType.Name;
            if (parameter.ParameterType.IsGenericType)
            {
                var arg = parameter.ParameterType.GenericTypeArguments[0].Name;
                if (parameterTypeName.Contains("Nullable"))
                {
                    arg = GetParameterTypeName(arg);
                    return arg + "?";
                }
                return arg;
            }
            return GetParameterTypeName(parameterTypeName);
        }

        public string GetParameterTypeName(string parameterTypeName)
        {
            if (parameterTypeName == "String")
            {
                return "string";
            }
            if (parameterTypeName == "Int32")
            {
                return "int";
            }
            if (parameterTypeName == "Boolean")
            {
                return "bool";
            }
            return parameterTypeName.Replace(sourceSuffix, destinationSuffix);
        }

        private string GetParameterDescription(MethodInfo method)
        {
            var controller = method.DeclaringType.Name;
            var name = "M:" + GetApiNameSpace(nameSpace) + ".Controllers." + controller + "." + method.Name;
            var xml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"" + GetApiNameSpace(nameSpace) + ".Controllers.XML");
            var doc = XDocument.Load(xml);
            XElement element = doc.XPathSelectElements("//member").FirstOrDefault(u => u.Attribute("name").Value.Contains(name));
            if (element == null)
            {
                return "";
            }

            var summary = element.Element("summary").Value;
            var paramList = element.Elements("param");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// " + summary.Trim());
            sb.AppendLine("        /// </summary>");
            foreach (var xElement in paramList)
            {
                sb.AppendFormat("        /// <param name=\"{0}\">{1}</param>", xElement.Attribute("name").Value.Replace(sourceParameterName, destinationParameterName), xElement.Value);
                sb.AppendLine();
            }
            sb.AppendLine("        /// <returns>");
            sb.AppendLine("        /// ");
            sb.AppendLine("        /// </returns>");
            sb.AppendLine("        /// 创建者：史鹏飞");
            sb.AppendLine("        /// 创建日期：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
            sb.AppendLine("        /// 修改者：");
            sb.AppendLine("        /// 修改时间：");
            sb.Append("        /// ----------------------------------------------------------------------------------------");




            return sb.ToString();
        }


        private bool CreateServiceConfig(List<Type> controllers)
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GenerateFold");

            var path = string.Format("{0}/Config/service.config", basePath);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<configuration>");
            foreach (var controller in controllers)
            {
                var serviceName = controller.Name.Replace("Controller", "Service");
                sb.AppendFormat("<component id=\"" + nameSpace + ".Services.{0}\"", serviceName);
                sb.AppendLine();
                sb.AppendFormat(
                    "   service=\"" + nameSpace + ".Controllers.IServices.I{0}, " + nameSpace + ".Controllers\"",
                    serviceName);
                sb.AppendLine();
                sb.AppendFormat("   type=\"" + nameSpace + ".Services.{0}, " + nameSpace + ".Services\">",
                    serviceName);
                sb.AppendLine();
                sb.AppendLine(" <interceptors>");
                sb.AppendLine("     <interceptor>${" + nameSpace + ".Interceptor.ServiceInterceptor}</interceptor>");
                sb.AppendLine(" </interceptors>");
                sb.AppendLine("</component>");
                sb.AppendLine();
            }
            sb.AppendLine("</configuration>");

            var text = sb.ToString();
            if (!CreateFile(path, text))
            {
                return false;
            }
            return true;
        }

        private bool CreateFile(string filePath, string text)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (directory == null)
            {
                Message = "创建文件失败，" + filePath;
                return false;
            }
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(text);
            }
            return true;
        }

        private List<Type> GetControllers(Assembly assembly)
        {
            var controllers = assembly.ExportedTypes
                .Where(u => u.BaseType != null && u.BaseType.Name.Contains("BaseController")).ToList();
            if (!controllers.Any())
            {
                Message = "没有符合的controller";
            }
            return controllers;
        }

        private Assembly GetAssembly()
        {
            return Assembly.Load(GetApiNameSpace(nameSpace) + ".Controllers");
        }

        private string GetApiNameSpace(string nameSpace)
        {
            return nameSpace.Replace(".Web", "").Replace(".Admin", "");
        }


    }
}
