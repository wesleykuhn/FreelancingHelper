using FreelancingHelper.Pages;
using FreelancingHelper.ViewModels;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FreelancingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //Setting the DataContext here because the main is not called by our navigation system
            MainViewModel viewModel = new();
            viewModel.BindedWindow = this;
            DataContext = viewModel;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var context = DataContext as MainViewModel;
            _ = Task.Run(() => context.InitAsync());
        }
    }
}
