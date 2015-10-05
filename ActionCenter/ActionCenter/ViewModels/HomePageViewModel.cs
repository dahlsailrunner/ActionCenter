using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ActionCenter.Events;
using ActionCenter.Services;
using NWP.ActionCenter.Entities;
using NWP.ActionCenter.Entities.Enums;
using NWP.ActionCenter.Entities.ServerToClientEntities;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Email;

namespace ActionCenter.ViewModels
{
    public class HomePageViewModel : NwpBindableBase, INavigationAware
    {
        private bool _isActionGrp;
        public bool IsActionGrp
        {
            get { return !IsLoading && _isActionGrp; }
            set { SetProperty(ref _isActionGrp, value); }
        }
        private ActionItem _swipedItem;
        public ActionItem SwipedItem
        {
            get { return _swipedItem; }
            set
            {
                SetProperty(ref _swipedItem, value);
                OnPropertyChanged("PrebillingSwiped");
                OnPropertyChanged("UtilityAlertSwiped");
            }
        }
        public bool PrebillingSwiped
        {
            get { return SwipedItem.ActionType == ActionItemType.PrebillingApproval; }
        }
        public bool UtilityAlertSwiped
        {
            get { return SwipedItem.ActionType == ActionItemType.UtilityAlert; }
        }
        private bool _isPrebillingSwiped;
        public bool IsPrebillingSwiped
        {
            get { return _isPrebillingSwiped; }
            set { SetProperty(ref _isPrebillingSwiped, value); }
        }
        private bool _isUtilityAlertSwiped;
        public bool IsUtilityAlertSwiped
        {
            get { return _isUtilityAlertSwiped; }
            set { SetProperty(ref _isUtilityAlertSwiped, value); }
        }
        private bool _isNotActionGrp;
        public bool IsNotActionGrp
        {
            get
            {
                return !IsLoading && _isNotActionGrp;
            }
            set { SetProperty(ref _isNotActionGrp, value); }
        }
        private bool _multiSelect;
        public bool MultiSelect
        {
            get { return _multiSelect; }
            set { SetProperty(ref _multiSelect, value); }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        private string _loadText;
        public string LoadText
        {
            get { return _loadText; }
            set { SetProperty(ref _loadText, value); }
        }
        private string _resolveStr;
        public string ResolveStr
        {
            get { return _resolveStr; }
            set { SetProperty(ref _resolveStr, value); }
        }
        private string _selectStr;
        public string SelectStr
        {
            get { return _selectStr; }
            set { SetProperty(ref _selectStr, value); }
        }
        private string _groupStr;
        public string GroupStr
        {
            get { return _groupStr; }
            set { SetProperty(ref _groupStr, value); }
        }
        private ObservableCollection<PropertyDetail> _properties;
        public ObservableCollection<PropertyDetail> Properties
        {
            get { return _properties; }
            set { SetProperty(ref _properties, value); }
        }
        private ObservableCollection<ActionItem> _actionItems;
        public ObservableCollection<ActionItem> ActionItems
        {
            get { return _actionItems; }
            set { SetProperty(ref _actionItems, value); }
        }
        private string _psrName;
        public string PsrName
        {
            get { return _psrName; }
            set { SetProperty(ref _psrName, value); }
        }
        private string _psrPhone;
        public string PsrPhone
        {
            get { return _psrPhone; }
            set { SetProperty(ref _psrPhone, value); }
        }
        private string _psrEmail;
        public string PsrEmail
        {
            get { return _psrEmail; }
            set { SetProperty(ref _psrEmail, value); }
        }
        private string _amName;
        public string AmName
        {
            get { return _amName; }
            set { SetProperty(ref _amName, value); }
        }
        private string _amPhone;
        public string AmPhone
        {
            get { return _amPhone; }
            set { SetProperty(ref _amPhone, value); }
        }
        private string _amEmail;
        public string AmEmail
        {
            get { return _amEmail; }
            set { SetProperty(ref _amEmail, value); }
        }
        public bool AreAnySelected
        {
            get
            {
                return ActionItems.Any(x => x.IsSelected);

            }
        }
        public bool AreAnyPrebillingSelected
        {
            get { return ActionItems.Any(x => x.IsSelected && x.ActionType == ActionItemType.PrebillingApproval); }
        }
        public bool AreAnyUtilitySelected
        {
            get { return ActionItems.Any(x => x.IsSelected && x.ActionType == ActionItemType.UtilityAlert); }
        }
        public DelegateCommand<string> PhoneCommand { get; set; }
        public DelegateCommand<string> EmailCommand { get; set; }
        public DelegateCommand HamburgerCommand { get; set; }
        public DelegateCommand GroupByCommand { get; set; }
        public DelegateCommand ForwardCommand { get; set; }

        public HomePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService, eventAggregator)
        {
            EventAgg.GetEvent<LoggedInEvent>().Subscribe(HandleLoginEvent);
            ActionItems = new ObservableCollection<ActionItem>();
            Properties = new ObservableCollection<PropertyDetail>();
            PhoneCommand = new DelegateCommand<string>(ExecPhoneCommand);
            EmailCommand = new DelegateCommand<string>(ExecEmailCommand);
            HamburgerCommand = new DelegateCommand(ExecHamburger);
            GroupByCommand = new DelegateCommand(ExecGroupBy);
            ForwardCommand = new DelegateCommand(ExecForward);
            IsLoading = false;
            MultiSelect = false;
            IsActionGrp = true;
            IsNotActionGrp = false;
            IsPrebillingSwiped = false;
            IsUtilityAlertSwiped = false;
            GroupStr = "Type";
            AmEmail = "Loading. . .";
            AmPhone = "Loading. . .";
            AmName = "Loading. . .";
            PsrEmail = "Loading. . .";
            PsrPhone = "Loading. . .";
            PsrName = "Loading. . .";
            SwipedItem = new ActionItem { ActionType = ActionItemType.PrebillingApproval };
            LoadText = "Inverting the flux capacitor. . .";
        }

        private void ExecForward()
        {
            var emailService = Resolver.Resolve<IEmailService>();
            var selectedItems = ActionItems.Where(i => i.IsSelected);

            var body = "";
            for (var i = 0; i < 4; i++)
            {
                body += Environment.NewLine + " ";
            }
            var c = 0;
            foreach (var selectedItem in selectedItems)
            {
                c++;
                body += string.Format("{0} ({1}) {2}", selectedItem.ActionTypeStr, selectedItem.InstanceId, Environment.NewLine);
            }
            var subject = string.Format("{0} unresolved Action Items regarding NWP Services Corporation", c);
            body += "";

            emailService.ShowDraft(subject, body, true, string.Empty);
        }
        private void ExecGroupBy()
        {
            IsActionGrp = !IsActionGrp;
            IsNotActionGrp = !IsNotActionGrp;
            GroupStr = IsActionGrp ? "Type" : "Property";
        }
        private void ExecHamburger()
        {
            NavService.Navigate("NwpSelectorPage");
        }
        public async void LoadData()
        {
            IsLoading = true;
            //App.SaveUtilityResponses();
            var actions = await NwpSmartApiService.GetActionItemsAsync();
            if (Properties.Count < 1)
            {
                var properties = await NwpSmartApiService.GetPropertiesAsync();
                LoadText = "Initiating self destruct sequence. . .";
                if (properties.Any())
                {
                    Properties = new ObservableCollection<PropertyDetail>(properties);
                    var i = 0;

                    while (i < properties.Count)
                    {
                        if (!string.IsNullOrWhiteSpace(properties[i].PropSvcRep)
                            && !string.IsNullOrWhiteSpace((properties[i].PsrPhone))
                            && !string.IsNullOrWhiteSpace((properties[i].PsrEmail))
                            && !string.IsNullOrWhiteSpace((properties[i].AcctManager))
                            && !string.IsNullOrWhiteSpace((properties[i].AcctMgrPhone))
                            && !string.IsNullOrWhiteSpace((properties[i].AcctMgrEmail)))
                            break;
                        i++;
                    }

                    PsrName = properties[i].PsrFirstName;
                    PsrPhone = properties[i].PsrPhone;
                    PsrEmail = properties[i].PsrEmail;
                    AmName = properties[i].AmFirstName;
                    AmPhone = properties[i].AcctMgrPhone;
                    AmEmail = properties[i].AcctMgrEmail;
                }
            }
            LoadText = "Approaching ludacris speed. . .";
            foreach (var actionItem in actions)
            {
                actionItem.PropertyChanged += ItemChanged;
            }
            ActionItems = new ObservableCollection<ActionItem>(actions);
            //var ds = DependencyService.Get<IBadgeNotification>();
            //ds.UpdateBadge(ActionItems.Count);
            //CrossBadge.Current.SetBadge(ActionItems.Count);

            IsLoading = false;
        }
        private void ItemChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("AreAnySelected");
            OnPropertyChanged("AreAnyPrebillingSelected");
            OnPropertyChanged("AreAnyUtilitySelected");

        }
        private void ExecPhoneCommand(string phoneNumber)
        {
            var device = Resolver.Resolve<IDevice>();
            device.PhoneService.DialNumber(phoneNumber);
        }
        private void ExecEmailCommand(string emailAddress)
        {
            var emailService = Resolver.Resolve<IEmailService>();
            emailService.ShowDraft("Subject Herro", "I am a body", true, emailAddress);
        }
        private void HandleLoginEvent(string nah)
        {
            LoadData();
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }
        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
        public void NavigateToActionItem(ActionItem actionItem)
        {
            if (actionItem == null) throw new ArgumentNullException("actionItem");
            var navParams = new NavigationParameters { { "ActionItem", actionItem } };
            switch (actionItem.ActionType)
            {
                case ActionItemType.MissingUtilityBill:
                    NavService.Navigate("MissingBillPage", navParams);
                    break;
                case ActionItemType.PrebillingApproval:
                    NavService.Navigate("PrebillingTabbedPage", navParams);
                    break;
                case ActionItemType.UtilityAlert:
                    NavService.Navigate("UtilityAlertPage", navParams);
                    break;
            }
        }
    }
}