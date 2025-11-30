using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FinalPOS
{
    public partial class frmVendor : Form
    {
        frmVendorList f;
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnection dbcon = new DBConnection();
        public frmVendor(frmVendorList f)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.f = f;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if(MessageBox.Show("Save this record ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("insert into tbl_Vendor (vendor, address, contactperson, phone, email) VALUES (@vendor, @address, @contactperson, @phone, @email)", cn);
                    cm.Parameters.AddWithValue("@vendor", txtvendor.Text);
                    cm.Parameters.AddWithValue("@address", txtaddress.Text);
                    cm.Parameters.AddWithValue("@contactperson", txtContactPerson.Text);
                    cm.Parameters.AddWithValue("@phone", txtTelephone.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Vendor Added Successfully", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    f.LoadRecords();
                }
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Clear()
        {
            txtvendor.Clear();
            txtaddress.Clear();
            txtContactPerson.Clear();
            txtTelephone.Clear();
            txtEmail.Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Update this record ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("update tbl_Vendor set vendor = @vendor , address =@address , contactperson = @contactperson, phone = @phone, email = @email ", cn);
                    cm.Parameters.AddWithValue("@vendor", txtvendor.Text);
                    cm.Parameters.AddWithValue("@address", txtaddress.Text);
                    cm.Parameters.AddWithValue("@contactperson", txtContactPerson.Text);
                    cm.Parameters.AddWithValue("@phone", txtTelephone.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.Parameters.AddWithValue("@id", lblID.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Vendor Updated Successfully ", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    f.LoadRecords();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
