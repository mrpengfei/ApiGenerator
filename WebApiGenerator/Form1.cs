using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

namespace WebApiGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetGeneratorList();
        }

        private void SetGeneratorList()
        {
            var root = GetConfigRoot();
            if (root==null)
            {
                return;
            }
            var generators = root.Elements("Generator").Select(u=>u.Attribute("name").Value).ToList();

           
            comboBox1.DataSource = generators;
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            var cofigName = comboBox1.Text;
            var root = GetConfigRoot();
            if (root==null)
            {
                MessageBox.Show("无效的配置文件");
                return;
            }

            var element = root.Elements("Generator").FirstOrDefault(u => u.Attribute("name").Value == cofigName);
            if (element==null)
            {
                MessageBox.Show("无效的选项");
                return;
            }

            GeneratorModel model = new GeneratorModel()
            {
                ModelDirectory = element.Element("modelDirectory").Value,
                SourceSuffix = element.Element("sourceSuffix").Value,
                DestinationSuffix = element.Element("destinationSuffix").Value,
                SourceParameterName = element.Element("sourceParameterName").Value,
                DestinationParameterName = element.Element("destinationParameterName").Value,
                NameSpace = element.Element("nameSpace").Value,
                ApiAddressPrefix = element.Element("apiAddressPrefix").Value,
            };

            GenerateService service = new GenerateService(model);
            service.Generate();
            System.Diagnostics.Process.Start(service.BasePath);
            System.Environment.Exit(0);
        }

        private XElement GetConfigRoot()
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/config.xml");
            if (!File.Exists(file))
            {
                return null;
            }
            XDocument doc = XDocument.Load(file);
            var root = doc.Root;
            return root;
        }
    }
}
