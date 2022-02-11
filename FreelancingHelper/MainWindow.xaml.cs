using System.Windows;
using System.Windows.Input;

namespace FreelancingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        //private void Window_StateChanged(object sender, System.EventArgs e)
        //{
        //    if (WindowState == WindowState.Minimized)
        //        return;
        //}
    }
}
