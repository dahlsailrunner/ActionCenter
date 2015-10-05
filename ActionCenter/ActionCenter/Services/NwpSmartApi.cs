using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using NWP.ActionCenter.Entities;
using NWP.ActionCenter.Entities.ServerToClientEntities;
using Xamarin.Forms;

namespace ActionCenter.Services
{
    public static class NwpSmartApiService
    {
        private static HttpClient ConfigureClient()
        {
            var client = new HttpClient(new NativeMessageHandler());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + App.Token);
            var url = Device.OnPlatform("http://actioncenterapp/ios", "http://actioncenterapp/android", "http://actioncenterapp/winphone");
            client.DefaultRequestHeaders.Referrer = new Uri(url);
            client.BaseAddress = new Uri(Constants.BaseAddress);
            //if (string.IsNullOrWhiteSpace(App.MaskedAuthority))
            //{
            //    client.DefaultRequestHeaders.Add(, App.MaskedAuthority);
            //}
            return client;
        }

        public static async Task<List<ResidentPrebillDetail>> GetResPrebillDetails(int propertyId, string actionInstId, double lowerBound, double upperBound)
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var queryString = "?propertyId=" + propertyId + "&actionInstId=" + actionInstId +
                        "&lowerBound=" + lowerBound + "&upperBound=" + upperBound;
                    var response = await client.GetAsync("actioncenter/resprebilldetails" + queryString);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var details = JsonConvert.DeserializeObject<List<ResidentPrebillDetail>>(json);
                    return details;
                }
            }
            catch (Exception e)
            {
                var x = e;
                return new List<ResidentPrebillDetail>();
            }

        }
        public static async Task<List<NamedEntity>> GetAllPortfoliosAsync()
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var response = await client.GetAsync("actioncenter/allportfolios");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var portfolios = JsonConvert.DeserializeObject<List<NamedEntity>>(json);
                    return portfolios;
                }
            }
            catch (Exception e)
            {
                var x = e;
                return new List<NamedEntity>();
            }
        }

        public static async Task<List<NamedEntity>> GetAllPropertiesAsync()
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var response = await client.GetAsync("actioncenter/allproperties");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var properties = JsonConvert.DeserializeObject<List<NamedEntity>>(json);
                    return properties;
                }
            }
            catch (Exception e)
            {
                var x = e;
                return new List<NamedEntity>();
            }
        }
        public static async Task PostRegistrationIdAsync(string registrationId)
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var payload = "?serviceName=" + Device.OnPlatform("iPhone", "Android", "WinPhone")
                                  + "&registrationId=" + WebUtility.UrlEncode(registrationId);
                    var response = await client.PostAsync("actioncenter/register" + payload, null);

                    if (response.StatusCode != HttpStatusCode.NoContent) throw new Exception("Registration failed");
                }
            }
            catch
            {
                // ignored
            }
        }
        public static async Task<List<ActionItem>> GetActionItemsAsync()
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var response = await client.GetAsync("actioncenter/actionitems");

                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var actions = JsonConvert.DeserializeObject<List<ActionItem>>(json);
                    return actions;
                }
            }
            catch
            {
                return null;
            }
        }
        public static async Task<Stream> GetPreBillingApprovalAsync(int propertyId, string perEndDt)
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var queryString = "?propertyId=" + propertyId + "&perEndDt=" + perEndDt;
                    var response = await client.GetAsync("actioncenter/billingpreviewreport" + queryString);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception)
            {
                //ignored
                return null;
            }
        }
        public static async Task<List<MissingBill>> GetMissingBillsAsync(string actionItemInstId)
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var queryString = "?instanceId=" + actionItemInstId;
                    var response = await client.GetAsync("actioncenter/missingbilldetails" + queryString);

                    var json = await response.Content.ReadAsStringAsync();
                    var bills = JsonConvert.DeserializeObject<List<MissingBill>>(json);
                    return bills;
                }
            }
            catch
            {
                return null;
            }
        }
        public static async Task<List<PropertyDetail>> GetPropertiesAsync()
        {

            using (var client = ConfigureClient())
            {
                var response = await client.GetAsync("actioncenter/getproperties");

                var json = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertyDetail>>(json);
                return properties;
            }
        }
        public static async void ApprovePrebillingAsync(string approvalInstId, int propertyId)
        {
            using (var client = ConfigureClient())
            {
                var queryString = "?approvalInstId=" + approvalInstId
                                  + "&propertyId=" + propertyId;
                var response = await client.PostAsync("actioncenter/approveprebilling" + queryString, null);

                if (response.StatusCode != HttpStatusCode.NoContent) throw new Exception("Approval Failed");
            }
        }
        public static async void RejectPrebillingAsync(string approvalInstId, int propertyId, string rejectionReason, string rejectionNotes)
        {
            var rejection = rejectionReason + " - " + rejectionNotes;
            using (var client = ConfigureClient())
            {
                var queryString = "?approvalInstId=" + approvalInstId
                                  + "&propertyId=" + propertyId
                                  + "&rejectionNotes=" + WebUtility.UrlEncode(rejection);
                var response = await client.PostAsync("actioncenter/rejectprebilling" + queryString, null);

                if (response.StatusCode != HttpStatusCode.NoContent) throw new Exception("Rejection Failed");
            }
        }

        public static async Task<string> GetPrebillingAllocationsAsync()
        {
            //TODO: 
            return "";
        }
        public static async Task<List<UtilityAlertResponseReason>> GetUtilityAlertResponseOptionsAsync()
        {
            using (var client = ConfigureClient())
            {
                var response = await client.GetAsync(Constants.BaseAddress + "actioncenter/getuaresponseoptions");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var responseReasons = JsonConvert.DeserializeObject<List<UtilityAlertResponseReason>>(json);
                return responseReasons;
            }
        }
        public static async Task<string> GetUtilityAlertNotes(string taskId)
        {
            using (var client = ConfigureClient())
            {
                var queryString = "?taskId=" + taskId;
                var response = await client.GetAsync("actioncenter/getnotesforalert" + queryString);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var notes = JsonConvert.DeserializeObject<string>(json);
                return notes;
            }
        }

        public static async void SaveUtilityAlertAsync(
            int dailyAlertId,
            string taskId,
            string selectedReasonId,
            string newNotes,
            bool closeTask)
        {
            using (var client = ConfigureClient())
            {
                var queryString = "?dailyAlertId=" + dailyAlertId + "&taskId=" + taskId +
                    "&selectedReasonId=" + selectedReasonId + "&newNotes=" + newNotes + "&closeTask=" + closeTask;
                var response = await client.PostAsync("actioncenter/saveutilityalert" + queryString, null);
                if (response.StatusCode == HttpStatusCode.NoContent) return;
            }
        }

        public static async Task<string> GetPdfUrlAsync(string url)
        {
            try
            {
                using (var client = ConfigureClient())
                {
                    var querystring = "?imgUrl=" + WebUtility.UrlEncode(url);
                    var response = await client.GetAsync("actioncenter/getpdfurlforimgurl" + querystring);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    var pdf = JsonConvert.DeserializeObject<string>(json);
                    return pdf;
                }
            }
            catch (Exception)
            {

                return "";
            }

        }
        public static async Task<LocalChart> GetChartDataAsync(string actionItemInstanceId, int propertyId)
        {
            if (actionItemInstanceId == null) throw new ArgumentNullException("actionItemInstanceId");

            try
            {
                using (var client = ConfigureClient())
                {
                    var queryString = "?propertyId=" + propertyId
                        + "&approvalInstId=" + actionItemInstanceId;
                    var response = await client.GetAsync("actioncenter/prebillingdetail" + queryString);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var chart = JsonConvert.DeserializeObject<PbChart>(json);
                    return new LocalChart(chart);
                }
            }
            catch (Exception)
            {

                var chart = new PbChart
                {
                    SupplierId = "1234141",
                    PerEndDt = "04132015"
                };
                var rng = new Random();
                for (var i = 0; i < rng.Next(60, 500); i++)
                {
                    var x = rng.Next(0, 100);
                    if (x <= 4)
                        chart.VacantCount++;
                    else if (x < 10)
                        chart.Count_0_10++;
                    else if (x < 20)
                        chart.Count_10_20++;
                    else if (x < 30)
                        chart.Count_20_30++;
                    else if (x < 40)
                        chart.Count_30_40++;
                    else if (x < 50)
                        chart.Count_40_50++;
                    else if (x < 60)
                        chart.Count_50_60++;
                    else if (x < 70)
                        chart.Count_GT60++;
                }
                chart.TotalCurrentCharges = rng.Next(0, 10000);
                chart.BillableCount = chart.Count_GT60 + chart.Count_50_60 + chart.Count_40_50 + chart.Count_30_40 + chart.Count_20_30 + chart.Count_10_20 + chart.Count_0_10;
                return new LocalChart(chart);
            }
        }
    }
    public class PbChart
    {
        public int BillableCount { get; set; }
        public int VacantCount { get; set; }
        public decimal TotalCurrentCharges { get; set; }
        public string SupplierId { get; set; }
        public string PerEndDt { get; set; }

        // ReSharper disable InconsistentNaming
        public int Count_0_10 { get; set; }
        public int Count_10_20 { get; set; }
        public int Count_20_30 { get; set; }
        public int Count_30_40 { get; set; }
        public int Count_40_50 { get; set; }
        public int Count_50_60 { get; set; }
        public int Count_GT60 { get; set; }
        // ReSharper restore InconsistentNaming

    }
    public class CategoricalData
    {
        public object Category { get; set; }
        public double Value { get; set; }
    }

    public class LocalChart
    {
        public PbChart Chart { get; set; }
        public List<CategoricalData> CategoricalData
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = "< $10", Value = Chart.Count_0_10},
                    new CategoricalData() {Category = "$10's", Value = Chart.Count_10_20},
                    new CategoricalData() {Category = "$20's", Value = Chart.Count_20_30},
                    new CategoricalData() {Category = "$30's", Value = Chart.Count_30_40},
                    new CategoricalData() {Category = "$40's", Value = Chart.Count_40_50},
                    new CategoricalData() {Category = "$50's", Value = Chart.Count_50_60},
                    new CategoricalData() {Category = "> $60", Value = Chart.Count_GT60}
                };
            }
        }

        public List<CategoricalData> CategoricalData0
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = "< $10", Value = Chart.Count_0_10}
                };
            }
        }

        public List<CategoricalData> CategoricalData1
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = "$10's", Value = Chart.Count_10_20}
                };
            }
        }

        public List<CategoricalData> CategoricalData2
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = "$20's", Value = Chart.Count_20_30}
                };
            }
        }

        public List<CategoricalData> CategoricalData3
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = "$30's", Value = Chart.Count_30_40}
                };
            }
        }

        public List<CategoricalData> CategoricalData4
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = "$40's", Value = Chart.Count_40_50}
                };
            }
        }

        public List<CategoricalData> CategoricalData5
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = "$50's", Value = Chart.Count_50_60}
                };
            }
        }

        public List<CategoricalData> CategoricalData6
        {
            get
            {
                return new List<CategoricalData>()
                {
                    new CategoricalData() {Category = " > $60", Value = Chart.Count_GT60}
                };
            }
        }


        public LocalChart(PbChart chart)
        {
            if (chart == null) throw new ArgumentNullException("chart");
            Chart = chart;
        }


    }
}
