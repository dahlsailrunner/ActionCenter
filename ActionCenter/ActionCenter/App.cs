using Xamarin.Forms;

namespace ActionCenter
{
    public class App : Application
    {
        public App()
        {
            var bs = new Bootstrapper();
            bs.Run(this);
        }

        public static string MaskedUserName { get; set; }

        public static void SaveAuthority(string authority)
        {
            MaskedUserName = authority;
        }

        public static int? MaskedPropertyId { get; set; }
        public static int? MaskedPortfolioId { get; set; }

        public static int BadgeNumber { get; set; }

        public static bool IsLoggedIn
        {
            get { return !string.IsNullOrWhiteSpace(Token); }
        }

        //public static async void SaveUtilityResponses()
        //{
        //    if (!IsLoggedIn) return;
        //    if (UtilityAlertResponseReasons != null) return;
        //    UtilityAlertResponseReasons = await NwpSmartApiService.GetUtilityAlertResponseOptionsAsync();
        //}
        //public static List<UtilityAlertResponseReason> UtilityAlertResponseReasons { get; private set; }
        public static string Token { get; private set; }

        public static void SaveToken(string token)
        {
            Token = token;
        }
    }
}
