

using FrontFitLife.Pages.UserControl;

namespace FrontFitLife
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("profile", typeof(UserProfilePage));
            Routing.RegisterRoute("userprofile", typeof(UserProfilePage));

        }
    }
}
