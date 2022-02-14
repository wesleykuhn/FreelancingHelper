using FreelancingHelper.ViewModels;

namespace FreelancingHelper.Models
{
    public class NavigationBagItem
    {
        public BaseViewModel ViewModel { get; set; }
        public BaseViewModel ParentViewModel { get; set; }

        public NavigationBagItem(BaseViewModel viewModel, BaseViewModel parentViewModel)
        {
            ViewModel = viewModel;
            ParentViewModel = parentViewModel;
        }
    }
}
