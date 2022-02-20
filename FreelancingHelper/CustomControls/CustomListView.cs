using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FreelancingHelper.CustomControls
{
    public class CustomListView : ListView
    {
        public static readonly DependencyProperty ItemDoubleClickedCommandProperty =
            DependencyProperty.Register(nameof(ItemDoubleClickedCommand), typeof(ICommand), typeof(CustomListView), new PropertyMetadata(null));

        public ICommand ItemDoubleClickedCommand
        {
            get => (ICommand)GetValue(ItemDoubleClickedCommandProperty);
            set => SetValue(ItemDoubleClickedCommandProperty, value);
        }

        public static readonly DependencyProperty ItemRightClickedCommandProperty =
            DependencyProperty.Register(nameof(ItemRightClickedCommand), typeof(ICommand), typeof(CustomListView), new PropertyMetadata(null));

        public ICommand ItemRightClickedCommand
        {
            get => (ICommand)GetValue(ItemRightClickedCommandProperty);
            set => SetValue(ItemRightClickedCommandProperty, value);
        }

        public CustomListView()
        {
            ItemContainerStyle = new(typeof(ListViewItem), App.Current.Resources["DefaultListViewItem"] as Style);
            ItemContainerStyle.Setters.Add(new EventSetter(MouseLeftButtonUpEvent, new MouseButtonEventHandler(ListViewItem_DoubleClicked)));
            ItemContainerStyle.Setters.Add(new EventSetter(MouseRightButtonUpEvent, new MouseButtonEventHandler(ListViewItem_RightClicked)));
        }

        private void ListViewItem_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            ItemDoubleClickedCommand?.Execute(sender);
        }

        private void ListViewItem_RightClicked(object sender, MouseButtonEventArgs e)
        {
            ItemRightClickedCommand?.Execute(sender);
        }
    }
}
