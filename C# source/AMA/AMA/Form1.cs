using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AMA
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            addDrives();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                label1.Text = "Default storage location: " + folderName;
                File.WriteAllText("settings.conf","PATH-" + folderName);
            }
        }

        private void addDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    comboBox1.Items.Add(drive.Name);
                }
            }
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = "Default storage location: " + ((ComboBox)sender).SelectedItem + "Movies";
            File.WriteAllText("settings.conf", "PATH-" + ((ComboBox)sender).SelectedItem + "Movies");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose(true);
            Application.Exit();
        }

    }
}
