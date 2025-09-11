using GYM_CLIENT.Model;
using GYM_CLIENT.View.Admin.Forms;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GYM_CLIENT.View.Admin
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : UserControl
    {
        public Client()
        {
            InitializeComponent();
        }

        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {

            AddClient client = new AddClient();
            client.ShowDialog();  

        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateClient client = new UpdateClient();
            client.ShowDialog();
        }
    }
}
