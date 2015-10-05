using System;
using ActionCenter.Droid;
using ActionCenter.Views;
using Android.App;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace ActionCenter.Droid
{
    public class LoginPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            var activity = Context as Activity;

            var auth = new OAuth2Authenticator(
                clientId: "actioncenterapp", // your OAuth2 client id
                scope: "actioncenter", // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri("https://idsat.nwpsmart.com/identity/connect/authorize"), // the auth URL for the service
                redirectUrl: new Uri("https://actioncenterapp/callback/")); // the redirect URL for the service

            auth.Completed += (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {
                    // Use eventArgs.Account to do wonderful things
                    App.SaveToken(eventArgs.Account.Properties["access_token"]);
                    //PushClient.Register(Context, "730398132449");
                    //var id = PushClient.GetRegistrationId(Context);
                    //App.SaveRegistrationId(id);
                    var page = Element as LoginPage;
                    if (page != null) page.NavigateToHome();
                }
                else
                {
                    // The user cancelled
                }
            };
            if (activity != null) activity.StartActivity(auth.GetUI(activity));
        }
    }
}