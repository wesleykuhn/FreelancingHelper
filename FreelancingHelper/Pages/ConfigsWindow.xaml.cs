using FreelancingHelper.ViewModels;
using System.Windows;
using System.Linq;

namespace FreelancingHelper.Pages
{
    /// <summary>
    /// Interaction logic for ConfigsWindow.xaml
    /// </summary>
    public partial class ConfigsWindow : BaseWindow
    {
        public ConfigsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConfigsViewModel vm)
            {
                var langsList = vm.Languages.ToList();
                ccbLanguages.SelectedIndex = langsList.FindIndex(fi => fi.Type == vm.SelectedLanguage.Type);
            }
        }
    }
}
