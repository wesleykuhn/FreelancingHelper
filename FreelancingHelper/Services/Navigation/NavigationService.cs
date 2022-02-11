using FreelancingHelper.Pages;
using FreelancingHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace FreelancingHelper.Services.Navigation
{
    public class NavigationService
    {
        private static Lazy<NavigationService> navigationLazy = new(() => new NavigationService());
        public static NavigationService Current => navigationLazy.Value;

        private readonly Dictionary<string, Type> _navigationMapping;

        public NavigationService()
        {
            _navigationMapping = new Dictionary<string, Type>();

            CreateNavigationMapping();
        }

        private void CreateNavigationMapping()
        {
            _navigationMapping.Add(nameof(ConfigsViewModel), typeof(ConfigsWindow));
        }

        public async Task ShowWindow<TViewModel>(object args = null) where TViewModel : BaseViewModel
        {
            var window = await CreateBindAndInitViewModel<TViewModel>(args);

            window.Show();
        }
        public async Task ShowWindowDialog<TViewModel>(object args = null) where TViewModel : BaseViewModel
        {
            var window = await CreateBindAndInitViewModel<TViewModel>(args);

            window.ShowDialog();
        }

        private async Task<Window> CreateBindAndInitViewModel<TViewModel>(object args)
        {
            var viewModel = CreateViewModel(typeof(TViewModel));
            var window = CreateWindow(typeof(TViewModel));
            await BindViewModelWithWindowAndInit(viewModel, window, args);

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

        private ValueTask BindViewModelWithWindowAndInit(BaseViewModel viewModel, Window window, object args)
        {
            window.DataContext = viewModel;
            viewModel.BindedWindow = window;

            return new ValueTask(viewModel.InitAsync(args));
        }
    }
}
