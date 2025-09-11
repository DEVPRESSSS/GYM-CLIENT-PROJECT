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
    /// Interaction logic for Staff.xaml
    /// </summary>
    public partial class Staff : UserControl
    {
        public Staff()
        {
            InitializeComponent();
        }

        private void StaffBtn_Click(object sender, RoutedEventArgs e)
        {
            AddStaff addStaff = new AddStaff();
            addStaff.ShowDialog();
            
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateStaff addStaff = new UpdateStaff();
            addStaff.ShowDialog();
        }
    }
}
