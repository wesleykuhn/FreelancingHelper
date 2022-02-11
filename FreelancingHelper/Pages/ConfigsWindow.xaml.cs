using System;
using System.Windows;
using System.Windows.Input;

namespace FreelancingHelper.Pages
{
    /// <summary>
    /// Interaction logic for ConfigsWindow.xaml
    /// </summary>
    public partial class ConfigsWindow : Window
    {
        public ConfigsWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
