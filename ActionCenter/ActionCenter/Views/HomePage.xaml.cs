using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionCenter.ViewModels;
using NWP.ActionCenter.Entities;
using NWP.ActionCenter.Entities.Enums;
using Telerik.XamarinForms.DataControls;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;

namespace ActionCenter.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void List_OnItemTapped(object sender, ItemTapEventArgs args)
        {
            var bc = BindingContext as HomePageViewModel;
            if (bc == null) return;
            var item = args.Item as ActionItem;
            if (item == null) return;
            bc.NavigateToActionItem(item);
        }

        private void ActionListView_OnItemSwiping(object sender, ItemSwipingEventArgs e)
        {
            var control = sender as RadListView;
            if (control == null) return;
            var item = e.Item as ActionItem;
            if (item == null) return;
            var bc = BindingContext as HomePageViewModel;
            if (bc == null) return;
            bc.IsUtilityAlertSwiped = item.ActionType == ActionItemType.UtilityAlert;
            bc.IsPrebillingSwiped = item.ActionType == ActionItemType.PrebillingApproval;
            bc.SwipedItem = item;
        }

        private void ActionListView_OnItemSwipeStarting(object sender, ItemSwipeStartingEventArgs e)
        {

        }

        private void ActionListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
        {
            var s = sender as RadListView;
            var bc = BindingContext as HomePageViewModel;
            if (bc == null || s == null) return;
            bc.LoadData();
            s.EndRefresh(true);
        }
    }
}
