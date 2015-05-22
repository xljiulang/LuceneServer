using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuceneLib;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuceneDemo
{    
    class Program
    {
        [STAThread]     
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        } 
    }
}
