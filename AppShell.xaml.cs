using AppMMR.Pages;

namespace AppMMR
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // 注册路由
            Routing.RegisterRoute("Work", typeof(WorkPage));
        }
    }
}
