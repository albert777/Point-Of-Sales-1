﻿using BondTech.HotkeyManagement.Win;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WindowsFormsApplication1;

namespace Point_Of_Sales
{
    public partial class FormPOS : Form
    {       
        private System.Windows.Forms.Button printButton;
        private Font printFont;
        private StreamReader streamToPrint;

        //HOTJKEY
        internal HotKeyManager MyHotKeyManager;
        #region **HotKeys
        LocalHotKey lhkNewInvoice = new LocalHotKey("lhkNewInvoice", Keys.F1);
        LocalHotKey lhkResetInvoice = new LocalHotKey("lhkResetInvoice", Keys.F2);
        LocalHotKey lhkItemInvoice = new LocalHotKey("lhkItemInvoice", Keys.F3);
        LocalHotKey lhkDeleteInvoice = new LocalHotKey("lhkDeleteInvoice", Keys.Delete);
        LocalHotKey lhkCashInvoice = new LocalHotKey("lhkCashInvoice", Keys.F4);
        LocalHotKey lhkDiscount = new LocalHotKey("lhkDiscount", Keys.F5);
        LocalHotKey lhkSaveInvoice = new LocalHotKey("lhkSaveInvoice", Keys.F6);
        #endregion
        public decimal m_Discount, totalAmountDiscount;
        public string m_MCode;
        CultureInfo culture = new CultureInfo("id-ID");
        clsFunctions sFunctions = new clsFunctions();
        public static FormPOS publicFormPOS;
        private static FormPOS sForm = null;
        public static FormPOS Instance()
        {
            if (sForm == null) { sForm = new FormPOS(); }
            return sForm;
        }

        MySqlDataAdapter daFormPOSList = new MySqlDataAdapter();
        MySqlCommand cmdAddInvoice;
        DataSet dsFormPOSList = new DataSet();

        int PosX, PosY;
        ListViewItem li;

        public FormPOS()
        {
            InitializeComponent();
            MyHotKeyManager = new HotKeyManager(this);
            MyHotKeyManager.LocalHotKeyPressed += new LocalHotKeyEventHandler(MyHotKeyManager_LocalHotKeyPressed);

            lhkNewInvoice.Enabled = true;
            lhkResetInvoice.Enabled = true;
            lhkItemInvoice.Enabled = true;
            lhkDeleteInvoice.Enabled = true;
            lhkCashInvoice.Enabled = true;
            lhkSaveInvoice.Enabled = true;
            lhkDiscount.Enabled = true;

            MyHotKeyManager.AddLocalHotKey(lhkNewInvoice);
            MyHotKeyManager.AddLocalHotKey(lhkResetInvoice);
            MyHotKeyManager.AddLocalHotKey(lhkItemInvoice);
            MyHotKeyManager.AddLocalHotKey(lhkDeleteInvoice);
            MyHotKeyManager.AddLocalHotKey(lhkCashInvoice);
            MyHotKeyManager.AddLocalHotKey(lhkSaveInvoice);
            MyHotKeyManager.AddLocalHotKey(lhkDiscount);
            //MyHotKeyManager.DisableOnManagerFormInactive = true;
        }

        void MyHotKeyManager_LocalHotKeyPressed(object sender, LocalHotKeyEventArgs e)
        {
            switch (e.HotKey.Name.ToLower())
            {
                case "lhknewinvoice":
                    btnNew.PerformClick();
                    break;
                case "lhkresetinvoice":
                    button1.PerformClick();
                    break;
                case "lhkiteminvoice":
                    btnProduct.PerformClick();
                    break;
                case "lhkdeleteinvoice":
                    btnDelete.PerformClick();
                    break;
                case "lhkcashinvoice":
                    btnBayar.PerformClick();
                    break;
                case "lhksaveinvoice":
                    btnSave.PerformClick();
                    break;
                case "lhkdiscount":
                   
                    break;
                default:
                    if (e.HotKey.Tag != null) System.Diagnostics.Process.Start((string)e.HotKey.Tag);
                    break;
            }
        }

        private void FormPOS_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            daFormPOSList = new MySqlDataAdapter("", clsConnection.CN);
            cmdAddInvoice = new MySqlCommand("INSERT INTO tblsales(invoiceno, productcode, nama_produk, unitprice, quantity,discount, subtotal, cash, changecash, dateadded)" +
                                               "VALUES (@getInvoice,@getProductID,@nama_produk,@getUnitPrice,@getQuantity,@getDiscount,@getSubTotal,@getCash,@getChange,@getDateAdded)", clsConnection.CN);
            NewInvoice();
            cmdAddInvoice.Parameters.Add("@getInvoice", MySqlDbType.VarChar);
            cmdAddInvoice.Parameters.Add("@getProductID", MySqlDbType.Int16);
            cmdAddInvoice.Parameters.Add("@nama_produk", MySqlDbType.VarChar);
            cmdAddInvoice.Parameters.Add("@getUnitPrice", MySqlDbType.Decimal);
            cmdAddInvoice.Parameters.Add("@getQuantity", MySqlDbType.Int16);
            cmdAddInvoice.Parameters.Add("@getDiscount", MySqlDbType.Int16);
            cmdAddInvoice.Parameters.Add("@getSubTotal", MySqlDbType.Decimal);
            cmdAddInvoice.Parameters.Add("@getCash", MySqlDbType.Decimal);
            cmdAddInvoice.Parameters.Add("@getChange", MySqlDbType.Decimal);
            //cmdAddInvoice.Parameters.Add("@getMemberCode", MySqlDbType.Int16);
            cmdAddInvoice.Parameters.Add("@getDateAdded", MySqlDbType.Date);
            //cmdAddInvoice.Parameters.Add("@getTotalAmount", MySqlDbType.Decimal);          
            //cmdAddInvoice.Parameters.Add("@getDiscount", MySqlDbType.Decimal);
            //cmdAddInvoice.Parameters.Add("@getAmountDiscount", MySqlDbType.Decimal);
            publicFormPOS = this;
        }

        void NewInvoice()
        {
            GenerateInvoice();
        }

        public Int32 UnixTimeStampUTC()
        {
            Int32 unixTimeStamp;
            DateTime currentTime = DateTime.Now;
            DateTime zuluTime = currentTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            unixTimeStamp = (Int32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }

        void GenerateInvoice()
        {
            //lblInvoice.Text = "NOMOR -" + clsFunctions.GenerateCD("SELECT MAX(autoid) FROM tblsales", "tblsales") + "/" + UnixTimeStampUTC();
            lblInvoice.Text = "" +UnixTimeStampUTC();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewInvoice();
            Reset();
            lvPOS.Items.Clear();
        }
        private void txtProductCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            //SetProductPOS("SELECT productcode, productname, unitprice, sellingprice, stock, autoid FROM tblproduct WHERE productcode='" + txtProductCode.Text + "'");
        }

        public void SetProductPOS(string sSQL)
        {
            try
            {
                long totalRow = 0;
                daFormPOSList.SelectCommand.CommandText = sSQL;
                dsFormPOSList.Clear();
                daFormPOSList.Fill(dsFormPOSList, "tblproduct");
                totalRow = dsFormPOSList.Tables["tblproduct"].Rows.Count - 1;
                txtProductCode.Text = dsFormPOSList.Tables["tblproduct"].Rows[0].ItemArray.GetValue(0).ToString();
                txtProductName.Text = dsFormPOSList.Tables["tblproduct"].Rows[0].ItemArray.GetValue(1).ToString();
                decimal mPrice = decimal.Parse(dsFormPOSList.Tables["tblproduct"].Rows[0].ItemArray.GetValue(2).ToString());
                txtPrice.Text = mPrice.ToString("C", culture);

                int totalQTY = 1;

                txtQTY.Text = totalQTY.ToString();
                
                txtStock.Text = dsFormPOSList.Tables["tblproduct"].Rows[0].ItemArray.GetValue(4).ToString();
                txtProductID.Text = dsFormPOSList.Tables["tblproduct"].Rows[0].ItemArray.GetValue(5).ToString();
                //Console.WriteLine(dsFormPOSList.Tables["tblproduct"].Rows[0].ItemArray.GetValue(0).ToString());

                txtdiscount.Text = dsFormPOSList.Tables["tblproduct"].Rows[0].ItemArray.GetValue(6).ToString();
                //txtSubTotal.Text = (decimal.Parse(txtQTY.Text) * decimal.Parse(CurToDec(txtPrice.Text))).ToString("c", culture);

                decimal mHargaQty = (decimal.Parse(txtQTY.Text) * decimal.Parse(CurToDec(txtPrice.Text)));
                decimal mTotalDiskon = (mHargaQty * decimal.Parse(txtdiscount.Text)) / 100;
                //(decimal.Parse(txtQTY.Text) * decimal.Parse(CurToDec(txtPrice.Text))).ToString("C", culture);


                txtSubTotal.Text = (mHargaQty - mTotalDiskon).ToString("C", culture);//(mHargaQty - m_Discount).ToString("C", culture);


                txtQTY.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Reset()
        {
            txtProductCode.Focus();
            txtProductCode.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtQTY.Text = string.Empty;
            txtSubTotal.Text = string.Empty;
            txtProductID.Text = string.Empty;
            m_Discount = 0;
            lblTotal.Text = "0";
            lblTotalAmount.Text = "0";
            lblDiskon.Text = "0%";
            lblChange.Text = "0";
            m_MCode = "";
            totalAmountDiscount = 0;
        }

        private void AddProductList()
        {
            if (lvPOS.FindItemWithText(txtProductCode.Text) != null)
            {
                MessageBox.Show("Produk Sudah Masuk..!", clsVariables.sMSGBOX, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
                return;
            }

            ListViewItem item = new ListViewItem(txtProductCode.Text, 1);
            item.SubItems.Add(txtProductName.Text);
            item.SubItems.Add(txtPrice.Text);
            item.SubItems.Add(txtQTY.Text);
            item.SubItems.Add(txtdiscount.Text);
            item.SubItems.Add(txtSubTotal.Text);
            item.SubItems.Add(txtProductID.Text);
            lvPOS.Items.Add(item);
            Reset();
            SumTotalAmount();
        }

        private string CurToDec(string m_Curr)
        {
            return m_Curr.Replace("Rp", "").Replace(".", "");
        }

        void SumTotalAmount()
        {
            SetDiscount();
        }

        private void txtQTY_TextChanged(object sender, EventArgs e)
        {
            if (txtQTY.Text != string.Empty)
            {
                if(txtdiscount.Text != string.Empty){

                    decimal mHargaQty = (decimal.Parse(txtQTY.Text) * decimal.Parse(CurToDec(txtPrice.Text)));
                    decimal mTotalDiskon = (mHargaQty * decimal.Parse(txtdiscount.Text)) / 100;
                    //(decimal.Parse(txtQTY.Text) * decimal.Parse(CurToDec(txtPrice.Text))).ToString("C", culture);


                    txtSubTotal.Text = (mHargaQty - mTotalDiskon).ToString("C", culture);//(mHargaQty - m_Discount).ToString("C", culture);
                }

                //(decimal.Parse(txtQTY.Text) * decimal.Parse(CurToDec(txtPrice.Text))).ToString("C", culture);


                //txtSubTotal.Text = (mHargaQty - m_Discount).ToString("C", culture);
            }

        }

        private void txtQTY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //JIKA STOCK MASIH
                if (int.Parse(txtQTY.Text) <= int.Parse(txtStock.Text))
                {
                    if (int.Parse(txtQTY.Text) != 0)
                    {
                        if (txtQTY.Text == string.Empty)
                            txtQTY.Text = "1";

                        AddProductList();
                    }
                    else
                    {
                        MessageBox.Show("Jumlah Quantity tidak boleh 0.", clsVariables.sMSGBOX, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Maaf, sisa stok tidak tersedia. Sisa stok adalah " + txtStock.Text + ".", clsVariables.sMSGBOX, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtQTY.Text = txtStock.Text;
                }
            }
        }

        public void SumCashFinish(string cash)
        {
            if (decimal.Parse(CurToDec(cash)) < decimal.Parse(CurToDec(lblTotalAmount.Text)))
            {
                MessageBox.Show("Maaf, nilai pembayaran lebih kecil dari nilai Total.", clsVariables.sMSGBOX, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblCash.Text = decimal.Parse(cash).ToString("C", culture);
                lblChange.Text = (decimal.Parse(CurToDec(cash)) - decimal.Parse(CurToDec(lblTotalAmount.Text))).ToString("C", culture);
            }
        }

        private void lvPOS_MouseUp(object sender, MouseEventArgs e)
        {
            if (lvPOS.Items.Count != 0)
            {
                if (li == null)
                    return;
                int subItemSelected = 3;
                int nStart = PosX;
                int spos = 0;
                int epos = lvPOS.Columns[1].Width;
                for (int i = 0; i < lvPOS.Columns.Count; i++)
                {
                    if (nStart > spos && nStart < epos)
                    {
                        subItemSelected = i;
                        break;
                    }

                    spos = epos;
                    epos += lvPOS.Columns[i].Width;
                }

                string value = li.SubItems[3].Text;
                if (InputBox("[ " + li.SubItems[0].Text + " ] " + li.SubItems[1].Text, "QTY [ " + li.SubItems[1].Text + " ] :", ref value) == DialogResult.OK)
                {
                    li.SubItems[3].Text = value;
                    //li.SubItems[4].Text = (decimal.Parse(li.SubItems[3].Text) * decimal.Parse(li.SubItems[2].Text)).ToString("C", culture);
                    SumTotalAmount();
                }
            }
        }

        private void btnBayar_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            FormCash sForm = new FormCash();
            sForm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
            lvPOS.Items.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int Jumlah_item = 0;
            for (int i = 0; i < this.lvPOS.Items.Count; i++)
            {
                //Jumlah_item = Jumlah_item + Convert.ToDecimal(lvPOS.Items[i].SubItems[3].Text.ToString());
                Jumlah_item = Jumlah_item + Convert.ToInt32(lvPOS.Items[i].SubItems[3].Text);
            }
            string path = string.Concat(Environment.CurrentDirectory, @"\laporan.txt");
            //TextWriter tw = new StreamWriter("D:\\laporan.txt");
            TextWriter tw = new StreamWriter(path);
            StringBuilder listViewContent = new StringBuilder();
            tw.WriteLine("BATIK BAYAT KLATEN");
            tw.WriteLine("TANGGAL :"+DateTime.Now.ToString());
            tw.WriteLine("-------------------------------------------------------------------------------------");
            tw.WriteLine("KODE BARANG  ".PadRight(16) + "NAMA BARANG ".PadRight(10) + "HARGA ".PadLeft(15) + "JUMLAH".PadLeft(15) + "TOTAL".PadLeft(10));
            tw.WriteLine("-------------------------------------------------------------------------------------");
            for (int item = 0; item < this.lvPOS.Items.Count; item++)
            {
                tw.WriteLine(lvPOS.Items[item].SubItems[0].Text.PadRight(14) + "  " +
                lvPOS.Items[item].SubItems[1].Text.PadRight(19) + "  " +
                lvPOS.Items[item].SubItems[2].Text.PadRight(15) + "  " +
                lvPOS.Items[item].SubItems[3].Text.PadRight(6) + "  " +
                lvPOS.Items[item].SubItems[4].Text.PadRight(5));
                tw.WriteLine(listViewContent);
                listViewContent = new StringBuilder();
            }
            tw.WriteLine("-------------------------------------------------------------------------------------");
            tw.WriteLine("-------------------------------------------------------------------------------------");
            tw.WriteLine("Jumlah Item :" +Jumlah_item.ToString().PadLeft(42));
            tw.WriteLine("Total Pembayaran :" + lblTotalAmount.Text.PadLeft(53).ToString());
            tw.Close();
            Code_Printer();      
            
                  
            //listViewPrinter printer = new listViewPrinter(lvPOS, new Point(70, 70), false, lvPOS.Groups.Count > 0, "NOTA PEMBELIAN");
            //printer.print();
            if (lvPOS.Items.Count > 0 && decimal.Parse(CurToDec(lblCash.Text)) > 0)
            {
                for (int i = 0; i < lvPOS.Items.Count; i++)
                {                 
                    /*int sum = 0;
                    int theWidth = lvPOS.Columns[i].Width;
                    int theHeight = lvPOS.Items[0].Bounds.Height;
                    li.Items[i].lvPOS.SubItems(sum + lvPOS.Bounds.X, lvPOS.Bounds.Y + theHeight * lvPOS.Items.Count,
                                            theWidth, theHeight);
                    sum += theWidth;*/

                    cmdAddInvoice.Parameters["@getInvoice"].Value = lblInvoice.Text;
                    //cmdAddInvoice.Parameters["@getProductID"].Value = int.Parse(lvPOS.Items[i].SubItems[5].Text);
                    cmdAddInvoice.Parameters["@getProductID"].Value = lvPOS.Items[i].SubItems[0].Text;
                    cmdAddInvoice.Parameters["@nama_produk"].Value = lvPOS.Items[i].SubItems[1].Text;
                    cmdAddInvoice.Parameters["@getUnitPrice"].Value = decimal.Parse(CurToDec(lvPOS.Items[i].SubItems[2].Text));
                    cmdAddInvoice.Parameters["@getQuantity"].Value = int.Parse(lvPOS.Items[i].SubItems[3].Text);
                    cmdAddInvoice.Parameters["@getDiscount"].Value = int.Parse(lvPOS.Items[i].SubItems[4].Text);
                    cmdAddInvoice.Parameters["@getSubTotal"].Value = decimal.Parse(CurToDec(lvPOS.Items[i].SubItems[5].Text));
                    cmdAddInvoice.Parameters["@getCash"].Value = decimal.Parse(CurToDec(lblCash.Text));
                    cmdAddInvoice.Parameters["@getChange"].Value = decimal.Parse(CurToDec(lblChange.Text));
                    //cmdAddInvoice.Parameters["@getMemberCode"].Value = m_MCode;
                    cmdAddInvoice.Parameters["@getDateAdded"].Value = DateTime.Now;
                    //cmdAddInvoice.Parameters["@getTotalAmount"].Value = decimal.Parse(CurToDec(lblTotalAmount.Text));                   
                    //cmdAddInvoice.Parameters["@getDiscount"].Value = decimal.Parse(CurToDec(lblInvoice.Text));
                    //cmdAddInvoice.Parameters["@getAmountDiscount"].Value = totalAmountDiscount;
                    cmdAddInvoice.ExecuteNonQuery();
                }
                Reset();
                lvPOS.Items.Clear();
                lblTotalAmount.Text = "0";
                lblTotal.Text = "0";
                lblCash.Text = "0";
                lblChange.Text = "0";
                NewInvoice();
                MessageBox.Show("TERIMA KASIH..!");
            }  
        }


        private void Code_Printer()
        {
            /*PRINTER***************************************************************************/
            try
            {
                string path = string.Concat(Environment.CurrentDirectory, @"\laporan.txt");
                //streamToPrint = new StreamReader("D:\\laporan.txt");
                streamToPrint = new StreamReader(path);
                try
                {
                    printFont = new Font("Consolas", 10);
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler
                       (this.halaman_printing);
                    pd.Print();
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            /*PRINTER***************************************************************************/
        }

        private void halaman_printing(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
            printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
            ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            FormProduct_View.sFormIndex = "POS";
            FormProduct_View sForm = new FormProduct_View();
            sForm.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvPOS.Items.Count > 0)
            {
                if (lvPOS.Items[lvPOS.FocusedItem.Index].Selected)
                {
                    lvPOS.Items[lvPOS.FocusedItem.Index].Remove();
                    SumTotalAmount();
                    txtProductCode.Focus();
                }
            }
        }

        private void lvPOS_DoubleClick(object sender, EventArgs e)
        {
            if (lvPOS.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = lvPOS.SelectedItems;
                ListViewItem lvItem = items[0];
                //string what = lvItem.Text;
                string value = lvItem.SubItems[3].Text;

                if (InputBox("[ " + lvItem.SubItems[0].Text + " ] " + lvItem.SubItems[1].Text, "QTY [ " + lvItem.SubItems[1].Text + " ] :", ref value) == DialogResult.OK)
                {
                    lvItem.SubItems[3].Text = value;
                    //lvItem.SubItems[4].Text = (decimal.Parse(CurToDec(lvItem.SubItems[3].Text)) * decimal.Parse(CurToDec(lvItem.SubItems[2].Text))).ToString("C", culture);
                    //SumTotalAmount();


                    decimal mHargaQty = (decimal.Parse(lvItem.SubItems[3].Text) * decimal.Parse(CurToDec(lvItem.SubItems[2].Text)));
                    decimal mTotalDiskon = (mHargaQty * decimal.Parse(lvItem.SubItems[4].Text)) / 100;
                    //(decimal.Parse(txtQTY.Text) * decimal.Parse(CurToDec(txtPrice.Text))).ToString("C", culture);


                    //txtSubTotal.Text = (mHargaQty - mTotalDiskon).ToString("C", culture);//(mHargaQty - m_Discount).ToString("C", culture);

                    lvItem.SubItems[5].Text = (mHargaQty - mTotalDiskon).ToString("C", culture);
                    //SumTotalAmount();


                    SetDiscount();
                }
            }
        }

        private void txtQTY_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) ||
            char.IsSymbol(e.KeyChar) ||
            char.IsWhiteSpace(e.KeyChar) ||
            char.IsPunctuation(e.KeyChar))
            e.Handled = true;
        }

        private void btnDiscount_Click(object sender, EventArgs e)
        {
           
        }

        public void SetDiscount(string mMemberCode = "")
        {
            try
            {
                long totalRow = 0;
                m_MCode = mMemberCode;
                if (mMemberCode != "")
                {
                    daFormPOSList.SelectCommand.CommandText = "SELECT tblmember.membercode, tblmember.fullname, tblmember.address, tblmember.telephone, tblmember.status, tblmember.discount, tbldiscount.percent FROM tbldiscount RIGHT JOIN tblmember ON tbldiscount.autoid = tblmember.discount WHERE tblmember.membercode LIKE '" + mMemberCode + "' ";
                    dsFormPOSList.Clear();
                    daFormPOSList.Fill(dsFormPOSList, "tblmember");
                    totalRow = dsFormPOSList.Tables["tblmember"].Rows.Count - 1;
                    m_Discount = decimal.Parse(dsFormPOSList.Tables["tblmember"].Rows[0].ItemArray.GetValue(6).ToString());
                }

                decimal sTotalAmount = 0;
                for (int x = 0; x < lvPOS.Items.Count; x++)
                {
                    sTotalAmount += decimal.Parse(CurToDec(lvPOS.Items[x].SubItems[5].Text));
                }

                decimal mRumusDiskon = (m_Discount / 100) * sTotalAmount;
                totalAmountDiscount = sTotalAmount - mRumusDiskon;

                lblTotalAmount.Text = totalAmountDiscount.ToString("C", culture);
                lblTotal.Text = lblTotalAmount.Text;

                lblDiskon.Text = m_Discount + "%";

                if (decimal.Parse(CurToDec(lblCash.Text)) > 0)
                {
                    lblCash.Text = decimal.Parse(CurToDec(lblCash.Text)).ToString("C", culture);
                    lblChange.Text = (decimal.Parse(CurToDec(lblCash.Text)) - decimal.Parse(CurToDec(lblTotalAmount.Text))).ToString("C", culture);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lblTotalAmount_Click(object sender, EventArgs e)
        {

        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            SetProductPOS("SELECT productcode, productname, unitprice, sellingprice, stock, autoid, discount FROM tblproduct WHERE productcode='" + txtProductCode.Text + "'");
        }

        private void lvPOS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }

}
