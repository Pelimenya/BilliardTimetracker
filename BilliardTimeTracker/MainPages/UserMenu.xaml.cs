using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace BilliardTimeTracker.MainPages;

public partial class UserMenu : Page
{
    public UserMenu()
    {
        InitializeComponent();
    }

    private void HamburgerMenuControl_OnItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs args)
    {
        var menuItem = (HamburgerMenuGlyphItem)args.ClickedItem;
        if (menuItem != null && menuItem.Label == "Выйти из аккаунта")
            NavigationService.Navigate(new LoginPage());
        TableMenu.SetCurrentValue(ContentProperty, args.ClickedItem);
        TableMenu.SetCurrentValue(HamburgerMenu.IsPaneOpenProperty, false);
    }
}