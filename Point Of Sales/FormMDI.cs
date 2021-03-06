﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Point_Of_Sales
{
    public partial class FormMDI : Form
    {
        private clsFunctions sFunctions = new clsFunctions();
        private bool isAdmin;

        public FormMDI()
        {
            InitializeComponent();
        }

        private void FormMDI_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "Selamat Datang ! : " + clsVariables.sFullname;
            //FormSplashScreen SplashScreen = new FormSplashScreen();
            //SplashScreen.ShowDialog();
            clsConnection conn = new clsConnection();
            conn.setConnection(clsVariables.sIPAddress, clsVariables.sDbUser, clsVariables.sDbName, clsVariables.sDbPassword);
            isAdmin = clsFunctions.recordExist("SELECT tblusers.usercode, tblusers.usertype FROM tblusers WHERE (tblusers.usertype=1 && tblusers.usercode='" + clsVariables.sUsercode + "') ORDER BY tblusers.autoid ASC", "tblusers");
            usersToolStripMenuItem.Enabled = isAdmin;
            supplierToolStripMenuItem.Enabled = isAdmin;
            rEPORTPENJUALANToolStripMenuItem.Enabled = isAdmin;
            rEPORTPEMBELIANToolStripMenuItem.Enabled = isAdmin;
        }

        void CloseAllChild()
        {
            foreach (Form child in this.MdiChildren)
            {
                if (!child.Focused)
                {
                    child.Close();
                }
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void keluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Tutup Aplikasi?", clsVariables.sMSGBOX, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            { e.Cancel = true; }
            else
            {
                e.Cancel = false;
                //clsUserLogs.record_logout(DateTime.Now.ToString(), clsVariables.sLibrarianID);
                clsConnection.CN.Close();
                MessageBox.Show(clsVariables.sFullname + "Exit..!", "Waktu: " + DateTime.Now.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void calculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsApp.shell("Calc.exe", "Calculator");
        }

        private void notepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsApp.shell("Notepad.exe", "Notepad");
        }

        private void toolBtnCalculator_Click(object sender, EventArgs e)
        {
            calculatorToolStripMenuItem.PerformClick();
        }

        private void toolBtnNotepad_Click(object sender, EventArgs e)
        {
            notepadToolStripMenuItem.PerformClick();
        }

        private void TimerDate_Tick(object sender, EventArgs e)
        {
            lblDate.Text = "Hari ini:  " + DateTime.Now.ToLongDateString() + " [ " + DateTime.Now.ToLongTimeString() + " ] ";
        }

        private void toolBtnLogout_Click(object sender, EventArgs e)
        {
            logoutToolStripMenuItem.PerformClick();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormUser.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void toolBtnSetting_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormSetting.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void toolBtnUsers_Click(object sender, EventArgs e)
        {
            usersToolStripMenuItem.PerformClick();
        }

        private void toolBtnSupplier_Click(object sender, EventArgs e)
        {
            supplierToolStripMenuItem.PerformClick();
        }

        private void supplierToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            CloseAllChild();
            Form sForm = FormSupplier.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
            //sForm.WindowState = FormWindowState.Normal;
        }

        private void itemCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormCategory.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
            //sForm.WindowState = FormWindowState.Maximized;
        }

        private void toolBtnCategory_Click(object sender, EventArgs e)
        {
            itemCategoryToolStripMenuItem.PerformClick();
        }

        private void productMasterFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormProduct.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void toolBtnProduct_Click(object sender, EventArgs e)
        {
            productMasterFileToolStripMenuItem.PerformClick();
        }

        private void pointOfSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormPOS.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void toolBtnPOS_Click(object sender, EventArgs e)
        {
            pointOfSalesToolStripMenuItem.PerformClick();
        }

        private void aboutFastaWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout mFrom = new FormAbout();
            mFrom.MdiParent = this;
            mFrom.Show();
        }

        private void toolBtnHelp_Click(object sender, EventArgs e)
        {
            aboutFastaWorldToolStripMenuItem.PerformClick();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormMember.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void barcodePrinterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Process.Start("C:\\Program Files (x86)\\Labeljoy 5\\Labeljoy5.exe");          
        }

        private void rEPORTPENJUALANToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();                   
            Form sForm = ReportPenjualan.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void rEPORTPEMBELIANToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = ReportPembelian.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void penjualanToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            rEPORTPENJUALANToolStripMenuItem.PerformClick();
        }

        private void pembelianToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            rEPORTPEMBELIANToolStripMenuItem.PerformClick();
        }

        private void produkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormProduct.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormMember.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();          
        }

        private void supplierToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormSupplier.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();
        }

        private void memberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllChild();
            Form sForm = FormMember.Instance();
            sForm.MdiParent = this;
            sForm.Show();
            sForm.Activate();          
        }
    }
}
