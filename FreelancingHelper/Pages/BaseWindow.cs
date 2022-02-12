using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FreelancingHelper.Pages
{
    public class BaseWindow : Window
    {
        private string[] _appsPreferredFontFamilies = new string[]
        {
            "Hack",
            "Calibri"
        };

        public BaseWindow()
        {
            ResizeMode = ResizeMode.CanMinimize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            MouseDown += BaseWindow_MouseDown;
            WindowStyle = WindowStyle.None;

            var fontFamily = TryGetFirstFontFamilyOfSequence(_appsPreferredFontFamilies);
            if (fontFamily != null)
                FontFamily = fontFamily;
        }

        private FontFamily TryGetFirstFontFamilyOfSequence(string[] fontFamilies)
        {
            foreach (var fontFamily in fontFamilies)
            {
                var result = TryGetFontFamily(fontFamily);

                if (result != null)
                    return result;
            }

            return null;
        }

        private FontFamily TryGetFontFamily(string fontFamily)
        {
            FontFamily result;

            try
            {
                result = new(fontFamily);

                return result;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        private void BaseWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
