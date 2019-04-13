using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;

namespace GST_BILL
{
    public partial class Form1 : Form
    {
        string loginCompanyName, loginGST, loginUsername;
        SqlConnection con;
        SqlDataAdapter adapter; 
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            loginVisible();
            
        }
        void position(Panel p)
        {
            p.Location = new Point(
                this.ClientSize.Width / 2 - p.Size.Width / 2,
                this.ClientSize.Height / 2 - p.Size.Height / 2);
            p.Anchor = AnchorStyles.None;
        }
        private void registrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            position(registration);
            registration.Visible = true;
            login.Visible = false;
            client.Visible = false;
            bill.Visible = false;
            product.Visible = false;
            s_register.Visible = true;
            update.Visible = false;
            s_name.Text = "";
            s_address.Text = "";
            s_gst.Text = "";
            bindState(s_state);
            s_username.Text = "";
            s_password.Text = "";
        }
        void loginVisible()
        {
            position(login);
            registration.Visible = false;
            login.Visible = true;
            client.Visible = false;
            bill.Visible = false;
            product.Visible = false;
            l_username.Text = "";
            l_password.Text = "";
        }
        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loginVisible();
        }
        private void addClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showClient();
            delete.Visible = false;
            addClient.Visible = true;
            updateClient.Visible = false;
            oldGSTLabel.Visible = false;
            oldGST.Visible = false;
        }
        void showClient()
        {
            position(client);
            registration.Visible = false;
            login.Visible = false;
            client.Visible = true;
            bill.Visible = false;
            product.Visible = false;
            c_name.Text = "";
            c_address.Text = "";
            c_gst.Text = "";
            bindState(c_state);
            
        }
        void bindGST()
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            SqlCommand cmd = new SqlCommand("SELECT gst FROM client WHERE username = @loginUsername ORDER BY gst ASC", con);
            cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet items = new DataSet();
            adapter.Fill(items);
            oldGST.DataSource = items.Tables[0];
            oldGST.DisplayMember = items.Tables[0].Columns["gst"].ToString();
            oldGST.ValueMember = items.Tables[0].Columns["gst"].ToString();
            con.Close();
        }
        private void billToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBill();
        }
        private void s_register_Click(object sender, EventArgs e)
        {
            if (s_name.Text.Equals("") || s_address.Text.Equals("") || s_gst.Text.Equals("") || s_state.Text.Equals("") || s_code.Text.Equals("") || s_username.Text.Equals("") || s_password.Text.Equals(""))
            {
                MessageBox.Show("All fields are Required");
            }
            else if(s_gst.Text.Length != 15)
            {
                MessageBox.Show("GST number is invalid");
            }
            else
            {
                con = new SqlConnection();
                con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM register WHERE username = @username", con);
                    cmd.Parameters.AddWithValue("@username", s_username.Text);
                    int valid = (int)cmd.ExecuteScalar();

                    if (valid != 0)
                        MessageBox.Show("Username Already Exist.");
                    else
                    {
                        cmd = new SqlCommand("INSERT INTO register VALUES (@companyName,@address,@gst,@state,@stateCode,@username,@password,@billCount)", con);
                        cmd.Parameters.AddWithValue("@companyName", s_name.Text);
                        cmd.Parameters.AddWithValue("@address", s_address.Text);
                        cmd.Parameters.AddWithValue("@gst", s_gst.Text);
                        cmd.Parameters.AddWithValue("@state", s_state.Text);
                        cmd.Parameters.AddWithValue("@stateCode", s_code.Text);
                        cmd.Parameters.AddWithValue("@username", s_username.Text);
                        cmd.Parameters.AddWithValue("@password", s_password.Text);
                        cmd.Parameters.AddWithValue("@billCount", "1");

                        int cnt = cmd.ExecuteNonQuery();
                        if (cnt == 1)
                        {
                            MessageBox.Show("You are registered successfully");
                            cmd = new SqlCommand("INSERT INTO product VALUES ('','',@username)", con);
                            cmd.Parameters.AddWithValue("@username", s_username.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();
                            loginVisible();
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                } 
            }

        }

        private void l_login_Click(object sender, EventArgs e)
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM register WHERE username=@username and password=@password", con);
           
                cmd.Parameters.AddWithValue("@username", l_username.Text);
                cmd.Parameters.AddWithValue("@password", l_password.Text);
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    loginCompanyName = sdr.GetString(0);
                    loginGST = sdr.GetString(2);
                    loginUsername = l_username.Text;
                    sdr.Close();
                    con.Close();
                    showBill();
                    logoutToolStripMenuItem.Visible = true;
                    loginToolStripMenuItem.Visible = false;
                    registrationToolStripMenuItem.Visible = false;
                    addClientToolStripMenuItem.Visible = true;
                    billToolStripMenuItem.Visible = true;
                    editInfoToolStripMenuItem.Visible = true;
                    editClientToolStripMenuItem.Visible = true;
                    addProductToolStripMenuItem.Visible = true;
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password");

                } 
            } 
        }
        
        void showBill()
        {
            position(bill);
            registration.Visible = false;
            login.Visible = false;
            client.Visible = false;
            bill.Visible = true;
            product.Visible = false;
            position(bill);
            company.Text = loginCompanyName;
            sender_gst.Text = loginGST;
            before_tax_value.Text = "0";
            after_tax_value.Text = "0";
            cgst_value.Text = "0";
            sgst_value.Text = "0";
            igst_value.Text = "0";
            cgst.SelectedIndex = 0;
            sgst.SelectedIndex = 0;
            igst.SelectedIndex = 0; 
            ComboBox[] products = { product1, product2, product3, product4, product5 };
            TextBox[] hsns = { hsn1, hsn2, hsn3, hsn4, hsn5 };
            NumericUpDown[] quantities = { quantity1, quantity2, quantity3, quantity4, quantity5 };
            NumericUpDown[] rates = { rate1, rate2, rate3, rate4, rate5 };
            TextBox[] values = { value1, value2, value3, value4, value5 };

            for (int i = 0; i < 5; i++)
            {
                products[i].Text = "";
                hsns[i].Text = "";
                quantities[i].Value = 0;
                rates[i].Value = 0;
                values[i].Text = "0";
            }
            date.Value = DateTime.Now; 
            bindCompanyName();
            bindProducts(0);
            bindProducts(1);
            bindProducts(2);
            bindProducts(3);
            bindProducts(4); 
            toggleEnable(true);
            newBill.Visible = false;
            save.Visible = true; 
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT billCount FROM register WHERE gst = @gst", con); 
            cmd.Parameters.AddWithValue("@gst", loginGST);
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                invoiceNo.Value = int.Parse(sdr.GetString(0));
                sdr.Close();
                con.Close();
            }
        }
        void bindCompanyName()
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            SqlCommand cmd = new SqlCommand("SELECT companyName FROM client WHERE username = @loginUsername ORDER BY companyName ASC", con);
            cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet items = new DataSet();
            adapter.Fill(items);
            b_name.DataSource = items.Tables[0];
            b_name.DisplayMember = items.Tables[0].Columns["companyName"].ToString();
            b_name.ValueMember = items.Tables[0].Columns["companyName"].ToString();
            con.Close();
        }

        void bindProducts(int i)
        {
            ComboBox[] products = { product1, product2, product3, product4, product5 };
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            SqlCommand cmd = new SqlCommand("SELECT name FROM product WHERE username = @loginUsername ORDER BY name ASC",con);
            cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet items = new DataSet();
            adapter.Fill(items);
            products[i].DataSource = items.Tables[0];
            products[i].DisplayMember = items.Tables[0].Columns["name"].ToString();
            products[i].ValueMember = items.Tables[0].Columns["name"].ToString();
            con.Close();
        }

        void bindState(ComboBox state)
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            adapter = new SqlDataAdapter("SELECT state FROM stateCode ORDER BY state ASC", con);
            DataSet items = new DataSet();
            adapter.Fill(items);
            state.DataSource = items.Tables[0];
            state.DisplayMember = items.Tables[0].Columns["state"].ToString();
            state.ValueMember = items.Tables[0].Columns["state"].ToString();
            con.Close();
        }

        private void addClient_Click(object sender, EventArgs e)
        {
            if (c_name.Text.Equals("") || c_address.Text.Equals("") || c_gst.Text.Equals("") || c_state.Text.Equals("") || c_code.Text.Equals(""))
            {
                MessageBox.Show("All fields are Required");
            }
            else if (c_gst.Text.Length != 15)
            {
                MessageBox.Show("GST number is invalid");
            }
            else
            {
                con = new SqlConnection();
                con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO client VALUES (@companyName,@address,@gst,@state,@stateCode,@loginUsername)", con);
                    cmd.Parameters.AddWithValue("@companyName", c_name.Text);
                    cmd.Parameters.AddWithValue("@address", c_address.Text);
                    cmd.Parameters.AddWithValue("@gst", c_gst.Text);
                    cmd.Parameters.AddWithValue("@state", c_state.Text);
                    cmd.Parameters.AddWithValue("@stateCode", c_code.Text);
                    cmd.Parameters.AddWithValue("@loginUsername", loginUsername);

                    int cnt = cmd.ExecuteNonQuery();
                    if (cnt == 1)
                    {
                        MessageBox.Show("Client added");
                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                con.Close();
                showClient();
            }
        }
        private void b_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            clientDetails();
        }
        void clientDetails()
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM client WHERE companyName = @companyName", con);
                cmd.Parameters.AddWithValue("@companyName", b_name.Text);
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    b_address.Text = sdr.GetString(1);
                    b_gst.Text = sdr.GetString(2);
                    b_state.Text = sdr.GetString(3);
                    b_code.Text = sdr.GetString(4);
                    sdr.Close();
                }
                con.Close();
            }
        }
        private void cgst_SelectedIndexChanged(object sender, EventArgs e)
        {
            cgst_value.Text = ((double.Parse(before_tax_value.Text) * double.Parse(cgst.Text)) / 100).ToString("F2");
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");
        }
        private void sgst_SelectedIndexChanged(object sender, EventArgs e)
        {
            sgst_value.Text = ((double.Parse(before_tax_value.Text) * double.Parse(sgst.Text)) / 100).ToString("F2");
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");
        }
        private void igst_SelectedIndexChanged(object sender, EventArgs e)
        {
            igst_value.Text = ((double.Parse(before_tax_value.Text) * double.Parse(igst.Text)) / 100).ToString("F2");
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");
        }
        private void rate1_ValueChanged(object sender, EventArgs e)
        {
            value1.Text = (quantity1.Value * rate1.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");
        }
        private void rate2_ValueChanged(object sender, EventArgs e)
        {
            value2.Text = (quantity2.Value * rate2.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");       
        }
        private void rate3_ValueChanged(object sender, EventArgs e)
        {
            value3.Text = (quantity3.Value * rate3.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");
        }
        private void rate4_ValueChanged(object sender, EventArgs e)
        {
            value4.Text = (quantity4.Value * rate4.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");

        }

        private void rate5_ValueChanged(object sender, EventArgs e)
        {
            value5.Text = (quantity5.Value * rate5.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");

        }
        private void quantity1_ValueChanged(object sender, EventArgs e)
        {
            value1.Text = (quantity1.Value * rate1.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");

        }
        private void quantity2_ValueChanged(object sender, EventArgs e)
        {
            value2.Text = (quantity2.Value * rate2.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");

        }
        private void quantity3_ValueChanged(object sender, EventArgs e)
        {
            value3.Text = (quantity3.Value * rate3.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");

        }
        private void quantity4_ValueChanged(object sender, EventArgs e)
        {
            value4.Text = (quantity4.Value * rate4.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");

        }

        private void quantity5_ValueChanged(object sender, EventArgs e)
        {
            value5.Text = (quantity5.Value * rate5.Value).ToString("F2");
            before_tax_value.Text = (double.Parse(value1.Text) + double.Parse(value2.Text) + double.Parse(value3.Text) + double.Parse(value4.Text) + double.Parse(value5.Text)).ToString();
            after_tax_value.Text = (double.Parse(before_tax_value.Text) + double.Parse(cgst_value.Text) + double.Parse(sgst_value.Text) + double.Parse(igst_value.Text)).ToString("F2");
        }
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logoutToolStripMenuItem.Visible = false;
            loginToolStripMenuItem.Visible = true;
            addClientToolStripMenuItem.Visible = false;
            billToolStripMenuItem.Visible = false;
            registrationToolStripMenuItem.Visible = true;
            editInfoToolStripMenuItem.Visible = false;
            editClientToolStripMenuItem.Visible = false;
            addProductToolStripMenuItem.Visible = false;
            loginVisible(); 
        }

        void toggleEnable(bool ch)
        {
            save.Visible = ch;
            newBill.Visible = !ch;
            date.Enabled = ch;
            b_name.Enabled = ch;
            cgst.Enabled = ch;
            igst.Enabled = ch;
            sgst.Enabled = ch;
            ComboBox[] products = { product1, product2, product3, product4, product5 };
            TextBox[] hsns = { hsn1, hsn2, hsn3, hsn4, hsn5 };
            NumericUpDown[] quantities = { quantity1, quantity2, quantity3, quantity4, quantity5 };
            NumericUpDown[] rates = { rate1, rate2, rate3, rate4, rate5 };
            for (int i = 0; i < 5; i++)
            {
                products[i].Enabled=ch;
                hsns[i].Enabled=ch;
                quantities[i].Enabled = ch;
                rates[i].Enabled = ch;
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            toggleEnable(false);
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM bill WHERE invoiceNo = @invoiceNo AND username = @loginUsername", con);
                cmd.Parameters.AddWithValue("@invoiceNo", invoiceNo.Value);
                cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    date.Text = sdr.GetString(1);
                    company.Text = sdr.GetString(2);
                    sender_gst.Text = sdr.GetString(3);
                    b_name.Text = sdr.GetString(4);
                    clientDetails();
                    string[] product, hsn, quantity, rate, value;
                    product = (sdr.GetString(6)).Split(':');
                    hsn = (sdr.GetString(7)).Split(':');
                    quantity = (sdr.GetString(8)).Split(':');
                    rate = (sdr.GetString(9)).Split(':');
                    value = (sdr.GetString(10)).Split(':');
                    ComboBox[] products = { product1, product2, product3, product4, product5 };
                    TextBox[] hsns = { hsn1, hsn2, hsn3, hsn4, hsn5 };
                    NumericUpDown[] quantities = { quantity1, quantity2, quantity3, quantity4, quantity5 };
                    NumericUpDown[] rates = { rate1, rate2, rate3, rate4, rate5 };
                    TextBox[] values = { value1, value2, value3, value4, value5 };
                    for (int i=0; i<5;i++)
                    {
                        products[i].Text = product[i];
                        hsns[i].Text = hsn[i];
                        quantities[i].Value = decimal.Parse(quantity[i]);
                        rates[i].Value = decimal.Parse(rate[i]);
                        values[i].Text = value[i];
                    }
                    before_tax_value.Text = sdr.GetString(11);
                    cgst.Text = sdr.GetString(12);
                    sgst.Text = sdr.GetString(13);
                    igst.Text = sdr.GetString(14);
                    cgst_value.Text = sdr.GetString(15);
                    sgst_value.Text = sdr.GetString(16);
                    igst_value.Text = sdr.GetString(17);
                    after_tax_value.Text = sdr.GetString(18);
                    sdr.Close();
                    con.Close();
                }
                else
                {
                    MessageBox.Show("Bill Not Found.");
                    con.Close();
                    showBill();
                } 
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }
        private void newBill_Click(object sender, EventArgs e)
        {
            showBill();
        }
        private void s_state_SelectedIndexChanged(object sender, EventArgs e)
        {
            stateDetails(s_state,s_code);
        }
        void stateDetails(ComboBox state,TextBox code)
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM stateCode WHERE state = @state", con);
                cmd.Parameters.AddWithValue("@state", state.Text);
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    code.Text = sdr.GetString(1);
                    sdr.Close();
                }
                con.Close();
            }
        }
        private void c_state_SelectedIndexChanged(object sender, EventArgs e)
        {
            stateDetails(c_state, c_code);
        } 
        private void save_Click(object sender, EventArgs e)
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM bill WHERE invoiceNo = @curInvoiceNo AND username = @loginUsername", con);
                decimal curInvoiceNo = invoiceNo.Value;
                cmd.Parameters.AddWithValue("@curInvoiceNo", curInvoiceNo.ToString());
                cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
                int count = (int)cmd.ExecuteScalar();
                if (count != 0)
                {
                    MessageBox.Show("Bill with this Invoice No. already exist.");
                    con.Close();
                }
                else
                {
                    cmd = new SqlCommand("INSERT INTO bill VALUES (@invoiceNo,@invoiceDate,@s_name,@s_gst,@r_name,@r_gst,@product,@hsn,@quantity,@rate,@value,@totalValueBeforeTax,@cgst,@sgst,@igst,@cgst_value,@sgst_value,@igst_value,@totalValueAfterTax,@loginUsername)", con);
                    cmd.Parameters.AddWithValue("@invoiceNO", invoiceNo.Text);
                    cmd.Parameters.AddWithValue("@invoiceDate", date.Text);
                    cmd.Parameters.AddWithValue("@s_name", company.Text);
                    cmd.Parameters.AddWithValue("@s_gst", sender_gst.Text);
                    cmd.Parameters.AddWithValue("@r_name", b_name.Text);
                    cmd.Parameters.AddWithValue("@r_gst", b_gst.Text); 
                    string product, hsn, quantity, rate, value;
                    product = product1.Text + ":" + product2.Text + ":" + product3.Text + ":" + product4.Text + ":" + product4.Text; 
                    cmd.Parameters.AddWithValue("@product", product); 
                    hsn = hsn1.Text + ":" + hsn2.Text + ":" + hsn3.Text + ":" + hsn4.Text + ":" + hsn5.Text;
                    cmd.Parameters.AddWithValue("@hsn", hsn); 
                    quantity = quantity1.Text + ":" + quantity2.Text + ":" + quantity3.Text + ":" + quantity4.Text + ":" + quantity5.Text;
                    cmd.Parameters.AddWithValue("@quantity", quantity); 
                    rate = rate1.Text + ":" + rate2.Text + ":" + rate3.Text + ":" + rate4.Text + ":" + rate5.Text;
                    cmd.Parameters.AddWithValue("@rate", rate); 
                    value = value1.Text + ":" + value2.Text + ":" + value3.Text + ":" + value4.Text + ":" + value5.Text;
                    cmd.Parameters.AddWithValue("@value", value); 
                    cmd.Parameters.AddWithValue("@totalValueBeforeTax", before_tax_value.Text);
                    cmd.Parameters.AddWithValue("@cgst", cgst.Text);
                    cmd.Parameters.AddWithValue("@sgst", sgst.Text);
                    cmd.Parameters.AddWithValue("@igst", igst.Text);
                    cmd.Parameters.AddWithValue("@cgst_value", cgst_value.Text);
                    cmd.Parameters.AddWithValue("@sgst_value", sgst_value.Text);
                    cmd.Parameters.AddWithValue("@igst_value", igst_value.Text);
                    cmd.Parameters.AddWithValue("@totalValueAfterTax", after_tax_value.Text);
                    cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
                    int cnt = cmd.ExecuteNonQuery();
                    if (cnt == 1)
                    {
                        curInvoiceNo++;
                        cmd = new SqlCommand("UPDATE register SET billCount = @curInvoiceNo WHERE gst = @gst", con);
                        cmd.Parameters.AddWithValue("@curInvoiceNo", curInvoiceNo.ToString());
                        cmd.Parameters.AddWithValue("@gst", loginGST);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Bill Entry done successfully");
                        showBill();
                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                con.Close();
            }
        }

        private void editInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            position(registration);
            registration.Visible = true;
            login.Visible = false;
            client.Visible = false;
            bill.Visible = false;
            product.Visible = false;
            s_register.Visible = false;
            update.Visible = true;
            bindState(s_state);
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM register WHERE gst = @loginGST", con);

                cmd.Parameters.AddWithValue("@loginGST", loginGST);
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    s_name.Text = sdr.GetString(0);
                    s_address.Text = sdr.GetString(1);
                    s_gst.Text = sdr.GetString(2);
                    s_state.Text = sdr.GetString(3);
                    s_code.Text = sdr.GetString(4);
                    s_username.Text = sdr.GetString(5);
                    s_password.Text = sdr.GetString(6);
                    sdr.Close();
                }
                con.Close();
            }
        }

        private void update_Click(object sender, EventArgs e)
        {
            if (s_name.Text.Equals("") || s_address.Text.Equals("") || s_gst.Text.Equals("") || s_state.Text.Equals("") || s_code.Text.Equals("") || s_username.Text.Equals("") || s_password.Text.Equals(""))
            {
                MessageBox.Show("All fields are Required");
            }
            else if (s_gst.Text.Length != 15)
            {
                MessageBox.Show("GST number is invalid");
            }
            else
            {
                con = new SqlConnection();
                con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM register WHERE username = @username", con);
                    cmd.Parameters.AddWithValue("@username", s_username.Text);
                    int valid = (int)cmd.ExecuteScalar();

                    if (!loginUsername.Equals(s_username.Text) && valid != 0)
                        MessageBox.Show("Username Already Exist.");
                    else
                    {

                        cmd = new SqlCommand("UPDATE register SET companyName=@companyName, address=@address, gst=@gst, state=@state, stateCode=@stateCode, username= @username, password=@password WHERE gst=@loginGST", con);
                        cmd.Parameters.AddWithValue("@companyName", s_name.Text);
                        cmd.Parameters.AddWithValue("@address", s_address.Text);
                        cmd.Parameters.AddWithValue("@gst", s_gst.Text);
                        cmd.Parameters.AddWithValue("@state", s_state.Text);
                        cmd.Parameters.AddWithValue("@stateCode", s_code.Text);
                        cmd.Parameters.AddWithValue("@username", s_username.Text);
                        cmd.Parameters.AddWithValue("@password", s_password.Text);
                        cmd.Parameters.AddWithValue("@loginGST", loginGST);

                        int cnt = cmd.ExecuteNonQuery();
                        if (cnt == 1)
                        {
                            MessageBox.Show("Updated Successfully");
                            cmd = new SqlCommand("UPDATE product SET username=@username WHERE username = @loginUsername", con);
                            cmd.Parameters.AddWithValue("@username", s_username.Text);
                            cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
                            cmd.ExecuteNonQuery();
                            cmd = new SqlCommand("UPDATE bill SET username=@username WHERE username = @loginUsername", con);
                            cmd.Parameters.AddWithValue("@username", s_username.Text);
                            cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
                            cmd.ExecuteNonQuery();
                            cmd = new SqlCommand("UPDATE client SET username=@username WHERE username = @loginUsername", con);
                            cmd.Parameters.AddWithValue("@username", s_username.Text);
                            cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
                            cmd.ExecuteNonQuery();
                            loginCompanyName = s_name.Text;
                            loginGST = s_gst.Text;
                            loginUsername = s_username.Text;
                            con.Close();
                            showBill();
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                }
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (c_gst.Text.Equals(""))
            {
                MessageBox.Show("GST number is required to delete");
            }
            else
            {
                con = new SqlConnection();
                con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM client WHERE gst = @gst", con);
                    cmd.Parameters.AddWithValue("@gst", c_gst.Text);

                    int cnt = cmd.ExecuteNonQuery();
                    if (cnt == 1)
                    {
                        MessageBox.Show("Client Deleted Successfully");
                    }
                    else
                    {
                        MessageBox.Show("GST number not found");
                    }
                }
                con.Close();
                showClient();
                bindGST();
            }
        }
        private void updateClient_Click(object sender, EventArgs e)
        {
            if (c_name.Text.Equals("") || c_address.Text.Equals("") || c_gst.Text.Equals("") || c_state.Text.Equals("") || c_code.Text.Equals("") || oldGST.Text.Equals(""))
            {
                MessageBox.Show("All fields are Required");
            }
            else if (c_gst.Text.Length != 15)
            {
                MessageBox.Show("GST number is invalid");
            }
            else
            {
                con = new SqlConnection();
                con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE client SET companyName = @companyName, address = @address, gst=@gst, state = @state, stateCode = @stateCode WHERE gst=@oldGST", con);
                    cmd.Parameters.AddWithValue("@companyName", c_name.Text);
                    cmd.Parameters.AddWithValue("@address", c_address.Text);
                    cmd.Parameters.AddWithValue("@gst", c_gst.Text);
                    cmd.Parameters.AddWithValue("@state", c_state.Text);
                    cmd.Parameters.AddWithValue("@stateCode", c_code.Text);
                    cmd.Parameters.AddWithValue("@oldGST", oldGST.Text);

                    int cnt = cmd.ExecuteNonQuery();
                    if (cnt == 1)
                    {
                        MessageBox.Show("Client details Updated");
                    }
                    else
                    {
                        MessageBox.Show("Old GST number is invalid.");
                    }
                }
                con.Close();
                showClient();
                bindGST();
            }
        }
        private void editClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showClient();
            bindGST();
            delete.Visible = true;
            addClient.Visible = false;
            updateClient.Visible = true;
            oldGST.Visible = true;
            oldGSTLabel.Visible = true;
        }
        private void oldGST_SelectedIndexChanged(object sender, EventArgs e)
        {
            details();
        }

        void details()
        {
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM client WHERE gst = @oldGST", con);

                cmd.Parameters.AddWithValue("@oldGST", oldGST.Text);
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    c_name.Text = sdr.GetString(0);
                    c_address.Text = sdr.GetString(1);
                    c_gst.Text = sdr.GetString(2);
                    c_state.Text = sdr.GetString(3);
                    c_code.Text = sdr.GetString(4);
                    sdr.Close();
                }
                con.Close();
            }
        }
        private void addProduct_Click(object sender, EventArgs e)
        {
            if (productName.Text.Equals("") || productHSN.Text.Equals(""))
            {
                MessageBox.Show("All fields are Required");
            }
            else if(productHSN.Text.Length != 6)
            {
                MessageBox.Show("Invalid HSN/ACS");
            }
            else
            {
                con = new SqlConnection();
                con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO product VALUES(@productName,@productHSN,@loginUsername)", con);
                    cmd.Parameters.AddWithValue("@productName", productName.Text);
                    cmd.Parameters.AddWithValue("@productHSN", productHSN.Text);
                    cmd.Parameters.AddWithValue("@loginUsername", loginUsername);
                    int cnt = cmd.ExecuteNonQuery();
                    if (cnt == 1)
                    {
                        MessageBox.Show("Product Added Successfully");
                        showProduct();
                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                con.Close();
            }
        }

        private void addProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showProduct();
        }

        void showProduct()
        {
            position(product);
            registration.Visible = false;
            login.Visible = false;
            client.Visible = false;
            bill.Visible = false;
            product.Visible = true;
            productName.Text = "";
            productHSN.Text = "";
        }

        private void productHSN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void product1_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHSN(1);
        }
        void showHSN(int index)
        {
            ComboBox[] products = { product1, product2, product3, product4, product5 };
            TextBox[] HSNs = { hsn1, hsn2, hsn3, hsn4, hsn5 };
            con = new SqlConnection();
            con.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = F:\net\OEP\GST BILL\GST BILL\gst.mdf; Integrated Security = True";
            if (con.State != ConnectionState.Open)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT hsn FROM product WHERE name = @product", con);
                cmd.Parameters.AddWithValue("@product", products[index-1].Text);
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    HSNs[index-1].Text = sdr.GetString(0);
                    sdr.Close();
                }
                con.Close();
            }
        }
        private void product2_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHSN(2);
        }

        private void product3_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHSN(3);
        }

        private void product4_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHSN(4);
        }

        private void product5_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHSN(5);
        }
        private void print_Click_1(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            printDialog.Document = printDocument; 
            printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(CreateBill);
            DialogResult result = printDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        public void CreateBill(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;
            Font font = new Font("Courier New", 12);
            SolidBrush brush = new SolidBrush(Color.Black);
            float fontHeight = font.GetHeight();
            int startX = 10;
            int startY = 10;
            int offset = 40;
            graphic.DrawString(loginCompanyName, new Font("Courier New", 18), brush, startX, startY);
            graphic.DrawString("GSTIN: " + loginGST, font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight;
            graphic.DrawString("____________________________________________________________________________________", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight + 5;
            graphic.DrawString("Date: " + date.Value.ToString("dd MMM yyyy"), font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("Invoice No.: " + invoiceNo.Value.ToString(), font, brush, startX, startY + offset);
            offset = offset + ((int)fontHeight*2);
            graphic.DrawString("Receiver's Details", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("------------------------------------------------------------------------------------", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight + 5; 
            graphic.DrawString("Comapany Name: " + b_name.Text, font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("Address: " + b_address.Text, font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("GSTIN: " + b_gst.Text, font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("State: " + b_state.Text, font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("State Code: " + b_code.Text, font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("------------------------------------------------------------------------------------", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight + 20; 
            graphic.DrawString("Product Details", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 
            graphic.DrawString("------------------------------------------------------------------------------------", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight + 5; 
            graphic.DrawString("Sr. No.  " + "   Product Name      " + "HSN/ACS   " + "  Quantity  " + "   Rate  " + "Taxable Value (Rs.)   ", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight; 

            ComboBox[] products = { product1, product2, product3, product4, product5 };
            TextBox[] hsns = { hsn1, hsn2, hsn3, hsn4, hsn5 };
            NumericUpDown[] quantities = { quantity1, quantity2, quantity3, quantity4, quantity5 };
            NumericUpDown[] rates = { rate1, rate2, rate3, rate4, rate5 };
            TextBox[] values = { value1, value2, value3, value4, value5 };

            for (int i=0; i<5; i++)
            {
                if (products[i].Text.Equals(""))
                    continue;
                else
                {
                    graphic.DrawString((i+1).ToString(), font, brush, startX, startY + offset);
                    graphic.DrawString(products[i].Text, font, brush, startX + 100, startY + offset);
                    graphic.DrawString(hsns[i].Text, font, brush, startX + 300, startY + offset);
                    graphic.DrawString(quantities[i].Value.ToString(), font, brush, startX + 430, startY + offset);
                    graphic.DrawString(rates[i].Value.ToString(), font, brush, startX + 550, startY + offset);
                    graphic.DrawString(values[i].Text, font, brush, startX + 650, startY + offset);
                    offset = offset + (int)fontHeight;
                }
            }
            graphic.DrawString("------------------------------------------------------------------------------------", font, brush, startX, startY + offset);
            offset = offset + (int)fontHeight + 5;
            graphic.DrawString("Totat Value Before Tax (Rs.)", font, brush, startX + 350, startY + offset);
            graphic.DrawString(before_tax_value.Text, font, brush, startX + 650, startY + offset);
            offset = offset + (int)fontHeight;
            if(!cgst_value.Text.Equals("0"))
            {
                graphic.DrawString("CGST", font, brush, startX + 430, startY + offset);
                graphic.DrawString(cgst.Text + "%", font, brush, startX + 500, startY + offset);
                graphic.DrawString(cgst_value.Text, font, brush, startX + 650, startY + offset);
                offset = offset + (int)fontHeight;
            }
            if (!sgst_value.Text.Equals("0"))
            {
                graphic.DrawString("SGST", font, brush, startX + 430, startY + offset);
                graphic.DrawString(sgst.Text + "%", font, brush, startX + 500, startY + offset);
                graphic.DrawString(sgst_value.Text, font, brush, startX + 650, startY + offset);
                offset = offset + (int)fontHeight;
            }
            if (!igst_value.Text.Equals("0"))
            {
                graphic.DrawString("IGST", font, brush, startX + 430, startY + offset);
                graphic.DrawString(igst.Text + "%", font, brush, startX + 500, startY + offset);
                graphic.DrawString(igst_value.Text, font, brush, startX + 650, startY + offset);
                offset = offset + (int)fontHeight;
            }
            graphic.DrawString("Totat Value After Tax (Rs.)", font, brush, startX + 350, startY + offset);
            graphic.DrawString(after_tax_value.Text, font, brush, startX + 650, startY + offset);
            offset = offset + (int)fontHeight;

        }
    }
}
