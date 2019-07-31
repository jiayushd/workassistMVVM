using WorkAssistMVVM.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace WorkAssistMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<KPI>();
            containerRegistry.RegisterForNavigation<KPIPlan>();
            containerRegistry.RegisterForNavigation<AttitudeScore>();
            containerRegistry.RegisterForNavigation<UCDonePointList>(); 
            containerRegistry.RegisterForNavigation<KPISummarize>();
            containerRegistry.RegisterForNavigation<UCTeamCase>();
        }

    }
}
