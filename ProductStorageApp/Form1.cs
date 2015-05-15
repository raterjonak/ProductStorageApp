using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductStorageApp
{
    public partial class ProductStorageUI : Form
    {
        private string connectionString =ConfigurationManager.ConnectionStrings["ProductStorageConString"].ConnectionString;
        
        Product productObj=new Product();
        private int quantity;
        public ProductStorageUI()
        {
            InitializeComponent();
        }



        private void saveButton_Click(object sender, EventArgs e)
        {
            productObj.code = int.Parse(productCodeTextBox.Text);
            productObj.description = productDescriptionTextBox.Text;
            productObj.quantity = int.Parse(productQuantityTextBox.Text);

            int codeLength = productCodeTextBox.Text.Length;
            int quantityValue = productObj.quantity;

            if (codeLength < 3 || quantityValue < 1)
            {
                MessageBox.Show(
                    "Sorry!!!!Your product code should be greater than 3 digit and quantity should be atleast 1 unit.");

            }


            else
            {

                if (IsproductCodeExist(productObj.code))
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    quantity = quantity + productObj.quantity;
                    string query = "UPDATE product_tbl SET code='" + productObj.code + "',description='" +
                                   productObj.description +
                                   "',quantity='" + quantity + "' WHERE code='" + productObj.code + "'";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    int rowUpdate = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowUpdate > 0)
                    {
                        MessageBox.Show("Update successful");
                        quantity = 0;

                    }

                    else
                    {
                        MessageBox.Show("Update operation fail!");
                    }
                }

                else
                {



                    SqlConnection connection = new SqlConnection(connectionString);
                    string query = "INSERT INTO product_tbl VALUES('" + productObj.code + "','" + productObj.description +
                                   "','" +
                                   productObj.quantity + "')";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Insertion successful");

                    }

                    else
                    {
                        MessageBox.Show("Insertion Fail!");
                    }
                }
            }
        }

        public bool IsproductCodeExist(int code)
        {
            bool isProductExist = false;
            SqlConnection connection=new SqlConnection(connectionString);
            string query = "Select * From product_tbl Where code='" + code + "'";
            SqlCommand command=new SqlCommand(query,connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Product>productsList=new List<Product>();

            while (reader.Read())
            {
                isProductExist = true;
                quantity = int.Parse(reader["quantity"].ToString());
                break;
            }

            return isProductExist;
        }

        public void ShowAllProduct()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM product_tbl";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Product> productList = new List<Product>();

            int totalQuantity=0;
            while (reader.Read())
            {
                Product product = new Product();
                product.code = int.Parse(reader["code"].ToString());
                product.description = reader["description"].ToString();
                product.quantity = int.Parse(reader["quantity"].ToString());
                productList.Add(product);

                totalQuantity += product.quantity;
            }

            LoadAllEmployeeListView(productList);
            totalTextBox.Text = totalQuantity.ToString();
        }

        public void LoadAllEmployeeListView(List<Product> products)
        {
            productListView.Items.Clear();
            foreach (var product in products)
            {
                ListViewItem item = new ListViewItem(product.code.ToString());
                item.SubItems.Add(product.description);
                item.SubItems.Add(product.quantity.ToString());
                

                productListView.Items.Add(item);
            }
        }

        private void showButton_Click(object sender, EventArgs e)
        {
            ShowAllProduct();

           
        }
    }
}
