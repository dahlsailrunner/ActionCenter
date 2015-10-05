using ActionCenter.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Xamarin.Forms;

namespace ActionCenter
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override Page CreateMainPage()
        {
            return Container.Resolve<HomePage>();
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<LoginPage>();
            //Container.RegisterTypeForNavigation<PrebillingPage>();
            ////Container.RegisterTypeForNavigation<ActionCenterPage>();
            //Container.RegisterTypeForNavigation<NwpSelectorPage>();
            //Container.RegisterTypeForNavigation<ResPrebillDetailsPage>();
            //Container.RegisterTypeForNavigation<UtilityAlertPage>();
            ////Container.RegisterTypeForNavigation<UtilityWebPage>();
            Container.RegisterTypeForNavigation<HomePage>();
            //Container.RegisterTypeForNavigation<PrebillingTabbedPage>();
            //Container.RegisterTypeForNavigation<PrebilledResidentsPage>();

        }
    }
}
