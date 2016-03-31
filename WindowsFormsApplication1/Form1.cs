using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DaiShu.Web.Model.Service.WebSiteConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string info = "{\"MerchantId\":1,\"MerchantName\":\"普慧添翼\",\"PaymentAccountType\":" +
            //              "{\"DictionaryId\":242,\"ParentCode\":\"DS052\",\"Name\":\"非账户托管\",\"Code\":\"DS05202\",\"Description\":null},\"Domain\":\"daishu360.com\"," +
            //              "\"SkinFrame\":\"second\",\"SkinColor\":\"blue\",\"SiteName\":\"贷鼠网\",\"HeadMetaList\":[\"<meta name=\"keywords\" content=\"贷鼠网,专业P2P平台\" />\"," +
            //              "\"<meta name=\"description\" content=\"贷鼠网是国内首批网络借贷平台之一,主要为全国工薪阶层、电商、中小企业主提供无担保无抵押贷款咨询服务。 \" />\"]}";
                        string info = "{\"MerchantId\":1,\"MerchantName\":\"普慧添翼\",\"PaymentAccountType\":" +
                          "{\"DictionaryId\":242,\"ParentCode\":\"DS052\",\"Name\":\"非账户托管\",\"Code\":\"DS05202\",\"Description\":null},\"Domain\":\"daishu360.com\"," +
                          "\"SkinFrame\":\"second\",\"SkinColor\":\"blue\",\"SiteName\":\"贷鼠网\",\"HeadMetaList\":[\"<meta name=\\\"keywords\\\" content=\\\"贷鼠网,专业P2P平台\\\" />\"," +
                          "\"<meta name=\\\"description\\\" content=\\\"贷鼠网是国内首批网络借贷平台之一,主要为全国工薪阶层、电商、中小企业主提供无担保无抵押贷款咨询服务。 \\\" />\"]}";

            var ss = JObject.Parse(info).ToObject<WebSiteConfig>();
        }
    }
}
