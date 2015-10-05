using ActionCenter.Events;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Xamarin.Forms;

namespace ActionCenter.Views
{
    public class LoginPage : ContentPage
    {
        private readonly IEventAggregator _eventAggregator;

        public LoginPage()
        {
            _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }
        public void NavigateToHome()
        {
            _eventAggregator.GetEvent<LoggedInEvent>().Publish("");
            Navigation.PopModalAsync();
        }
    }
}
