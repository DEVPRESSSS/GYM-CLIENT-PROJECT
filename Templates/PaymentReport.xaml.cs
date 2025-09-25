﻿using GYM_CLIENT.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;


namespace GYM_CLIENT.Template
{
    /// <summary>
    /// Interaction logic for PaymentReport.xaml
    /// </summary>
    public partial class PaymentReport : Window
    {

        IEnumerable<PaymentModel> _models;
        public PaymentReport(IEnumerable<PaymentModel> models)
        {
            InitializeComponent();
            _models = models;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ClientsPayment.ItemsSource = _models;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var mainBorder = MainBorder;

            if (mainBorder == null)
            {
                MessageBox.Show("Could not find the main border control!");
                return;
            }
            Submit.Visibility = Visibility.Hidden;

            // Create a fixed document for the preview
            FixedDocument document = new FixedDocument();

            // Save current layout settings
            Thickness originalMargin = mainBorder.Margin;
            mainBorder.UpdateLayout();

            // Create a page for the document
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            // Create a container for the entire UI visual
            Canvas container = new Canvas();
            container.Width = mainBorder.ActualWidth;
            container.Height = mainBorder.ActualHeight;

            // Use a visual brush to capture the entire UI
            VisualBrush vb = new VisualBrush(mainBorder);
            Rectangle rect = new Rectangle();
            rect.Width = mainBorder.ActualWidth;
            rect.Height = mainBorder.ActualHeight;
            rect.Fill = vb;
            container.Children.Add(rect);

            // Add the container to the page
            fixedPage.Children.Add(container);
            fixedPage.Width = mainBorder.ActualWidth;
            fixedPage.Height = mainBorder.ActualHeight;
            ((IAddChild)pageContent).AddChild(fixedPage);
            document.Pages.Add(pageContent);

            // Create a window with document viewer
            Window previewWindow = new Window();
            previewWindow.Title = "Print Preview";
            previewWindow.Width = 800;
            previewWindow.Height = 600;
            previewWindow.Owner = Window.GetWindow(this);
            DocumentViewer viewer = new DocumentViewer();
            viewer.Document = document;
            previewWindow.Content = viewer;
            previewWindow.ShowDialog();

            // Restore the original settings
            mainBorder.Margin = originalMargin;
            mainBorder.UpdateLayout();

            this.Close();

        }
    }
}
