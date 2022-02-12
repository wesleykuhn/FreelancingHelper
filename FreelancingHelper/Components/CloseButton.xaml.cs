using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FreelancingHelper.Components
{
    /// <summary>
    /// Interaction logic for CloseButton.xaml
    /// </summary>
    public partial class CloseButton : UserControl
    {
        public CloseButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(CloseButton));

        public ICommand ClickCommand
        {
            get => (ICommand)GetValue(ClickCommandProperty);
            set => SetValue(ClickCommandProperty, value);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClickCommand.Execute(null);
        }
    }
}
