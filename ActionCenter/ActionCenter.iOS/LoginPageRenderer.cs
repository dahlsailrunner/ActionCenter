using System;
using ActionCenter.iOS;
using ActionCenter.Views;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace ActionCenter.iOS
{
    public class LoginPageRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            //HACK
            if (App.IsLoggedIn) return;

            base.ViewDidAppear(animated);
            var auth = new OAuth2Authenticator(
                clientId: "actioncenterapp", // your OAuth2 client id
                scope: "actioncenter", // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri("https://idsat.nwpsmart.com/identity/connect/authorize"), // the auth URL for the service
                redirectUrl: new Uri("https://idsat.nwpsmart.com/identity")); // the redirect URL for the service
            auth.Completed += async (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {
                    App.SaveToken(eventArgs.Account.Properties["access_token"]);
                    await DismissViewControllerAsync(true);
                    var page = Element as LoginPage;
                    page?.NavigateToHome();
                }
                else
                {
                    // The user cancelled
                }
            };
            PresentViewController(auth.GetUI(), true, null);
        }
    }
}