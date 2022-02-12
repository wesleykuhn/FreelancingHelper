using System.Windows;
using System.Windows.Controls;

namespace FreelancingHelper.Components
{
    /// <summary>
    /// Interaction logic for SectionPanel.xaml
    /// </summary>
    public partial class SectionPanel : UserControl
    {
        public SectionPanel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SectionNameProperty =
            DependencyProperty.Register(nameof(SectionName), typeof(string), typeof(SectionPanel), new PropertyMetadata(string.Empty));

        public string SectionName
        {
            get => (string)GetValue(SectionNameProperty);
            set => SetValue(SectionNameProperty, value);
        }
    }
}
