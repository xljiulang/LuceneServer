using LuceneLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuceneDemo
{
    public partial class QueryForm : Form
    {
        private Lucene<News> client;

        public QueryForm(Lucene<News> client)
        {
            InitializeComponent();
            this.client = client;
        }

        /// <summary>
        /// 取最新一条
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private async Task<LnPage<News>> SearchIndex_Test(string keywords)
        {
            return await client
                .SearchIndex(keywords)
                .MatchField(item => item.Title, "red")
                .MatchField(item => item.Content, Color.Red, 100)
                .OrderByDescending(item => item.OrderIndex)
                .Skip(0)
                .Take(1)
                .ToPage();
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            var keywords = this.textBox1.Text.Trim();
            if (keywords.Length == 0)
            {
                return;
            }

            var page = await this.SearchIndex_Test(keywords);
            this.propertyGrid1.SelectedObject = page.FirstOrDefault();
        }
    }
}
