using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using System.Linq;

namespace Daz_Content_Installer_V0._1
{
    public partial class Form2 : Form
    {
        string newRuntimeFolder;
        string newRTShortname;
        public static SQLiteConnection connection;
        public Form2(SQLiteConnection dbConnection)
        {
            InitializeComponent();
            connection = dbConnection;
        }


        public void BxRuntimeFolder_TextChanged(object sender, EventArgs e)
        {
            newRuntimeFolder = BxRuntimeFolder.Text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (newRuntimeFolder == "")
            {
                //Pop up empty runtime location message
            }
            if (newRTShortname == "")
            {
                //Pop up empty shortname message
            }
            else
            {
                AddRuntimeRecord(newRTShortname, newRuntimeFolder);
            }
        }



        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void BtnRuntimeBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = RuntimeBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get the selected folder path and update bxOutDir
                newRuntimeFolder = RuntimeBrowserDialog.SelectedPath;
                BxRuntimeFolder.Text = newRuntimeFolder;
            }
        }

        private void BxRuntimeShortName_TextChanged(object sender, EventArgs e)
        {
            newRTShortname = ((TextBox)sender).Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ChkDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkDelete.Checked)
            {
                BtnDelRuntime.Visible = true;
            }
            else if (!ChkDelete.Checked)
            {
                BtnDelRuntime.Visible = false;
            }
        }

        
    }
}
