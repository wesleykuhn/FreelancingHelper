using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FreelancingHelper.CustomControls
{
    public class CustomComboBox : ComboBox
    {
        public static readonly DependencyProperty SelectedChangedCommandProperty =
            DependencyProperty.Register(nameof(SelectedChangedCommand), typeof(ICommand), typeof(CustomComboBox), new PropertyMetadata(null));

        public ICommand SelectedChangedCommand
        {
            get => (ICommand)GetValue(SelectedChangedCommandProperty);
            set => SetValue(SelectedChangedCommandProperty, value);
        }

        public CustomComboBox()
        {
            SelectionChanged += new SelectionChangedEventHandler(OnItemSelectedChanged);
        }

        //You can either use this calling and send the ComboBox selected item as param and use a var in the VM to control the selected item.
        private void OnItemSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null)
                return;

            SelectedChangedCommand?.Execute((sender as ComboBox).SelectedItem);
        }
    }
}
