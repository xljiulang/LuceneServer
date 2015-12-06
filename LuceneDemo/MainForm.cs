using LuceneLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuceneDemo
{
    public partial class MainForm : Form
    {
        private Lucene<News> client = new Lucene<News>("Index_News");

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            var state = await this.client.Connect(IPAddress.Loopback, 8088);
            this.Text = "连接服务器" + (state ? "成功" : "失败");

            if (state)
            {
                var logined = await this.client.Login("admin", "123456");
            }
            else
            {
                this.btnSet.Enabled = this.btnQuery.Enabled = false;
            }
        }

        private async Task<bool> SetIndex_Test(string title, string content)
        {
            var news = new News
            {
                Id = Guid.Parse("{B0A0F268-28E3-4151-9DAC-F970C6991C79}"),
                OrderIndex = Environment.TickCount,
                CreateTime = DateTime.Now,
                Title = title,
                Content = content
            };
            return await client.SetIndex(news);
        }

        private async void btnSet_Click(object sender, EventArgs e)
        {
            var state = await this.SetIndex_Test(this.textBox1.Text, this.textBox2.Text);
            MessageBox.Show("设置索引" + (state ? "成功" : "失败"), "设置索引");
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            var queryFrom = new QueryForm(this.client);
            queryFrom.ShowDialog();
        }
    }
}
