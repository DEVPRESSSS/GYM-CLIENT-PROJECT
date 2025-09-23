using GYM_CLIENT.DatabaseConnection;
using GYM_CLIENT.Model;
using Microsoft.Data.SqlClient;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using static QRCoder.PayloadGenerator.SwissQrCode;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;

namespace GYM_CLIENT.View.Admin.Forms
{
    /// <summary>
    /// Interaction logic for AccessCardView.xaml
    /// </summary>
    public partial class AccessCardView : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public AccessCardView()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        string? cardNumber = "";
        string? clientId = "";
        private void GenerateQR_Click(object sender, RoutedEventArgs e)
        {
            string cardNumber = $"F&G-{Guid.NewGuid().ToString().ToUpper().Substring(0, 6)}";
            string qrFilePath = "";
            try
            {
                var qrImage = GenerateQRCode(clientId);
                string relativePath = SaveQRCodeFromBitmapImage(qrImage, cardNumber);
                QRCodePictureBox.Source = qrImage;

                string query = "INSERT INTO AccessCards (CardNumber,ClientId,QrCode) VALUES(@CardNumber,@ClientId,@QrCode)";
                sqlConnection.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@CardNumber", cardNumber);
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    cmd.Parameters.AddWithValue("@QrCode", relativePath);
                    cmd.ExecuteNonQuery();
                    
                }

                string accessCardUpdate = "UPDATE Client SET hasAccessCard = @hasAccessCard WHERE ClientId = @ClientId";

                using (SqlCommand cmd = new SqlCommand(accessCardUpdate, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@hasAccessCard", 1); 
                    cmd.Parameters.AddWithValue("@ClientId", clientId);

                    cmd.ExecuteNonQuery();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}");
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

        public BitmapImage GenerateQRCode(string data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Generate as Bitmap
            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                return BitmapToImageSource(qrCodeImage);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); 
                return bitmapImage;
            }
        }

        private string SaveQRCodeFromBitmapImage(BitmapImage qrImage, string cardNumber)
        {
            try
            {
                string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

                string projectFolder = Path.Combine(projectRoot, "QRcode");

                if (!Directory.Exists(projectFolder))
                {
                    Directory.CreateDirectory(projectFolder);
                }

                string fileName = $"QR_{cardNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(projectFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(qrImage));
                    encoder.Save(fileStream);
                }

                return Path.Combine("QRcode", fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving QR code: {ex.Message}");
                return "Error saving file";
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var infoclient = DataContext as ClientModel;

            if (infoclient == null) return;
            cardNumber = $"{Guid.NewGuid().ToString().Substring(0, 8)}";
            clientId = infoclient.ClientId;

            if(infoclient.hasAccessCard != true)
            {
                GenerateQR.IsEnabled = true;

            }
            else
            {
                GenerateQR.IsEnabled = false;
                GenerateQR.Visibility = Visibility.Hidden;

            }

            try
            {

                string query = "SELECT QrCode FROM AccessCards WHERE ClientId=@ClientId";
                sqlConnection.Open();

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@ClientId", clientId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string relativePath = reader["QrCode"].ToString();

                            string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
                            string absolutePath = Path.Combine(projectRoot, relativePath);

                            QRCodePictureBox.Source = new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                        }
                    }
                }






            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}");
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                    sqlConnection.Close();
            }


        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
