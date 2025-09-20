using GYM_CLIENT.DatabaseConnection;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace GYM_CLIENT.View.Admin.Forms
{
    /// <summary>
    /// Interaction logic for AddEquipments.xaml
    /// </summary>
    public partial class AddEquipments : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public event EventHandler equipmentCreated;

        public AddEquipments()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            InsertEquipment();
        }

        string relativePath = "";
        private void ChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string absolutePath = openFileDialog.FileName;

                string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

                string projectFolder = Path.Combine(projectRoot, "Images");
                if (!Directory.Exists(projectFolder))
                    Directory.CreateDirectory(projectFolder);

                string fileName = Path.GetFileName(absolutePath);
                string destinationPath = Path.Combine(projectFolder, fileName);

                File.Copy(absolutePath, destinationPath, true);

                relativePath = Path.Combine("Images", fileName);

                string absoluteImagePath = Path.Combine(projectFolder, fileName);
                EquipmentPicture.ImageSource = new BitmapImage(new Uri(absoluteImagePath, UriKind.Absolute));
            }

        }


        private void InsertEquipment()
        {
            string query = "INSERT INTO Equipments (EquipmentId,Name,Stat,Quantity,ImageUrl)VALUES(@EquipmentId,@Name,@Stat,@Quantity,@ImageUrl)";

            try
            {
                sqlConnection.Open();




                if (string.IsNullOrWhiteSpace(Name.Text) || (string.IsNullOrWhiteSpace(Quantity.Text)))
                {
                    MessageBox.Show($"All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {

                    cmd.Parameters.AddWithValue("@EquipmentId", $"EQUIPMENT-{Guid.NewGuid().ToString().ToUpper().Substring(0, 10)}");
                    cmd.Parameters.AddWithValue("@Name", Name.Text);

                    if(int.TryParse(Quantity.Text, out var quantity))
                    {
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                    }
                    else
                    {
                        MessageBox.Show($"Quantity is not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Clear();
                        return;

                    }
                    cmd.Parameters.AddWithValue("@Stat", 1);
                    cmd.Parameters.AddWithValue("@ImageUrl", relativePath);


                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        MessageBox.Show("Equipment inserted successfully");
                        Clear();
                        equipmentCreated?.Invoke(this, new EventArgs());
                        sqlConnection.Close();
                        this.Close();
                    }



                }

             }
            catch
            {
                MessageBox.Show($"Name is already exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Clear();
                sqlConnection.Close();

            }

}

        private void Clear()
        {

            Name.Text = "";
            Quantity.Text = "";
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.AllowOnlyNumbers(sender, e);

        }

        private void Quantity_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.ValidationHelper.NoSpaceOnly(sender, e);

        }

        private void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.PersonNameTextComposition(sender, e);

        }

        private void Name_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.ValidationHelper.PersonNameTextKeyDown(sender, e);

        }
    }
}
