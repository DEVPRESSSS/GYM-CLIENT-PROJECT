using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

using System.IO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GYM_CLIENT.View.Admin.Forms
{
    /// <summary>
    /// Interaction logic for UpdateEquipment.xaml
    /// </summary>
    public partial class UpdateEquipment : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public event EventHandler equipmentUpdated;
        public UpdateEquipment()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        string relativePath = "";
        private void ChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                string absolutePath = openFileDialog.FileName;

                string projectFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(projectFolder))
                    Directory.CreateDirectory(projectFolder);

                string fileName = Path.GetFileName(absolutePath);
                string destinationPath = Path.Combine(projectFolder, fileName);

                File.Copy(absolutePath, destinationPath, true);

                relativePath = GetRelativePath(destinationPath, AppDomain.CurrentDomain.BaseDirectory);
                EquipmentPicture.ImageSource = new BitmapImage(new Uri(relativePath, UriKind.Relative));

            }
        }
        private string GetRelativePath(string absolutePath, string basePath)
        {
            if (!basePath.EndsWith("\\"))
                basePath += "\\";

            Uri baseUri = new Uri(basePath);
            Uri absoluteUri = new Uri(absolutePath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(absoluteUri).ToString().Replace('/', '\\'));
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            UpdateEquipmentRecord();
        }
        private void UpdateEquipmentRecord()
        {
            string query = "UPDATE Equipments SET Name = @Name, Quantity = @Quantity, ImageUrl = @ImageUrl, Stat = @Stat WHERE EquipmentId = @EquipmentId";

            try
            {
                sqlConnection.Open();

                if (string.IsNullOrWhiteSpace(Name.Text) || string.IsNullOrWhiteSpace(Quantity.Text))
                {
                    MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@EquipmentId", equipmentId);
                    cmd.Parameters.AddWithValue("@Name", Name.Text);

                    if (int.TryParse(Quantity.Text, out var quantity))
                    {
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                    }
                    else
                    {
                        MessageBox.Show("Quantity is not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Clear();
                        return;
                    }

                    cmd.Parameters.AddWithValue("@ImageUrl", relativePath);

                    // Convert ComboBox selection to BIT (1 = Available, 0 = Not Available)
                    string selectedStatus = (Availability.SelectedItem as ComboBoxItem)?.Content.ToString();
                    bool statValue = selectedStatus == "Available"; // true if Available, false otherwise
                    cmd.Parameters.AddWithValue("@Stat", statValue);

                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        MessageBox.Show("Equipment updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Clear();
                        equipmentUpdated?.Invoke(this, new EventArgs());
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
            }
        }


        string? equipmentId = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var equipments = DataContext as EquipmentModel;

            if(equipments != null)
            {
                equipmentId= equipments.EquipmentId;
                Name.Text = equipments.Name;
                Quantity.Text = equipments.Quantity;
                if (!string.IsNullOrWhiteSpace(equipments.ImageUrl))
                {
                    EquipmentPicture.ImageSource = new BitmapImage(
                        new Uri(equipments.ImageUrl, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    EquipmentPicture.ImageSource = null; // or a default image
                }

            }


        }
        private void Clear()
        {

            Name.Text = "";
            Quantity.Text = "";
        }

        private void Name_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            HelperValidation.ValidationHelper.UsernameTextComposition(sender, e);

        }

        private void Name_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void Quantity_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

            HelperValidation.ValidationHelper.AllowOnlyNumbers(sender, e);




        }

        private void Quantity_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            HelperValidation.ValidationHelper.NoSpaceOnly(sender, e);

        }
    }
}
