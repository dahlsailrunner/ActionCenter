using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;

namespace ActionCenter.ViewModels
{
    public abstract class NwpBindableBase : BindableBase
    {
        public readonly INavigationService NavService;
        public readonly IEventAggregator EventAgg;

        public NwpBindableBase(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            NavService = navigationService;
            EventAgg = eventAggregator;

            if (!App.IsLoggedIn)
            {
                NavService.Navigate("LoginPage");
            }
        }
    }
}
