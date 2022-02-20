using FreelancingHelper.Models;
using FreelancingHelper.Pages;
using FreelancingHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace FreelancingHelper.Services.Navigation
{
    public class NavigationService
    {
        private static Lazy<NavigationService> navigationLazy = new(() => new NavigationService());
        public static NavigationService Current => navigationLazy.Value;

        private readonly Dictionary<string, Type> _navigationMapping;

        private List<NavigationBagItem> _navigationBag;

        public NavigationService()
        {
            _navigationMapping = new Dictionary<string, Type>();

            CreateNavigationMapping();
        }

        // If this options is enabled we setup our navigation bag, like in mobile apps, but without stack order.
        // Adds the first viewModel (wich represents our page), in this case the main one, that doesnt have a parent
        public void TrySetupNavigationStack(BaseViewModel mainViewModel)
        {
            
            if (!ConstantsAndSettings.EnableBagNavigation)
                return;

            _navigationBag = new List<NavigationBagItem>();
            _navigationBag.Add(new(mainViewModel, null));
        }

        private void CreateNavigationMapping()
        {
            _navigationMapping.Add(nameof(ConfigsViewModel), typeof(ConfigsWindow));
            _navigationMapping.Add(nameof(HirersManagerViewModel), typeof(HirersManagerWindow));
            _navigationMapping.Add(nameof(AddEditHirerViewModel), typeof(AddEditHirerWindow));
            _navigationMapping.Add(nameof(HistoryViewModel), typeof(HistoryWindow));
            _navigationMapping.Add(nameof(DayWorkDetailsViewModel), typeof(DayWorkDetailsWindow));
        }

        public async Task GoAsync<TViewModel>(object args = null, [CallerFilePath]string newVmParentPath = "") where TViewModel : BaseViewModel
        {
            if (!CheckIfNewWindowCanBeOpen(typeof(TViewModel)))
                return;

            HandlViewModelParentPath(ref newVmParentPath);

            var window = await CreateBindAndInitViewModel<TViewModel>(args, newVmParentPath);

            window.Show();
        }

        public async Task GoDialogAsync<TViewModel>(object args = null, [CallerFilePath]string newVmParentPath = "") where TViewModel : BaseViewModel
        {
            if (CheckIfNewWindowCanBeOpen(typeof(TViewModel)))
                return;

            HandlViewModelParentPath(ref newVmParentPath);

            var window = await CreateBindAndInitViewModel<TViewModel>(args, newVmParentPath);

            window.ShowDialog();
        }
        public ValueTask BackAsync(BaseViewModel currentViewModel, object args = null)
        {
            if (ConstantsAndSettings.EnableBagNavigation)
            {
                var viewModelIndex = _navigationBag.FindIndex(fi => fi.ViewModel.GetType().Name == currentViewModel.GetType().Name);

                var bagItem = _navigationBag[viewModelIndex];

                _navigationBag.RemoveAt(viewModelIndex);

                currentViewModel.BindedWindow.Close();

                return new ValueTask(bagItem.ParentViewModel.BackAsync(args));
            }
            else
                currentViewModel.BindedWindow.Close();
        }

        private bool CheckIfNewWindowCanBeOpen(Type viewModelType)
        {
            if (!ConstantsAndSettings.EnableBagNavigation)
                return true;

            return !_navigationBag.Exists(e => e.ViewModel.GetType() == viewModelType);
        }

        private void HandlViewModelParentPath(ref string newVmParentPath)
        {
            if (string.IsNullOrEmpty(newVmParentPath) || !ConstantsAndSettings.EnableBagNavigation)
            {
                newVmParentPath = null;
                return;
            }

            var barIndex = newVmParentPath.LastIndexOf('\\');
            var parentName = newVmParentPath.Substring(barIndex + 1, newVmParentPath.Length - (barIndex + 1)).Replace(".cs", "");
            newVmParentPath = parentName;
        }

        private async Task<Window> CreateBindAndInitViewModel<TViewModel>(object args, string parentViewModelName)
        {
            var viewModel = CreateViewModel(typeof(TViewModel));
            var window = CreateWindow(typeof(TViewModel));

            await BindViewModelWithWindowAndInit(viewModel, window, args, parentViewModelName);

            return window;
        }

        private BaseViewModel CreateViewModel(in Type viewModelType) =>
            (BaseViewModel)Activator.CreateInstance(viewModelType);

        private Window CreateWindow(in Type viewModelType)
        {
            var windowType = GetWindowTypeFromMappings(viewModelType.Name);
            return (Window)Activator.CreateInstance(windowType);
        }

        private Type GetWindowTypeFromMappings(in string viewModelName)
        {
            if (!_navigationMapping.ContainsKey(viewModelName))
                throw new Exception($"No map for {viewModelName} was found on navigation mappings!");

            return _navigationMapping[viewModelName];
        }

        private ValueTask BindViewModelWithWindowAndInit(BaseViewModel viewModel, Window window, object args, string parentViewModelName)
        {
            window.DataContext = viewModel;
            viewModel.BindedWindow = window;

            if (ConstantsAndSettings.EnableBagNavigation)
            {
                var newVmParentIndex = _navigationBag.FindIndex(fi => fi.ViewModel.GetType().Name == parentViewModelName);

                _navigationBag.Add(new(viewModel, _navigationBag[newVmParentIndex].ViewModel));
            }

            return new ValueTask(viewModel.InitAsync(args));
        }
    }
}
