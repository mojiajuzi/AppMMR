using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using AppMMR.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using AppMMR.Pages;
using AppMMR.Entities;
using Microsoft.EntityFrameworkCore;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AppMMR
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var dbPath = Constants.DatabasePath;
                options.UseSqlite($"Filename={dbPath};Foreign Keys=False");
            }, ServiceLifetime.Scoped);

            //项目详情相关    
            builder.Services.AddTransient<WorkPaymentFormViewModel>();
            builder.Services.AddTransient<WorkPaymentFormPage>();

            builder.Services.AddTransient<WorkContactFormViewModel>();
            builder.Services.AddTransient<WorkContactFormPage>();

            builder.Services.AddTransient<WorkContactViewModel>();
            builder.Services.AddTransient<WorkContactPage>();

            builder.Services.AddTransient<WorkPaymentViewModel>();
            builder.Services.AddTransient<WorkPaymentPage>();

            builder.Services.AddTransient<WorkDetailViewModel>();
            builder.Services.AddTransient<WorkDetailPage>();

            builder.Services.AddTransient<WorkTabViewModel>();
            builder.Services.AddTransient<WorkTabPage>();

            //项目相关
            builder.Services.AddTransient<WorkFormViewModel>();
            builder.Services.AddTransient<WorkFormPage>();
            builder.Services.AddTransient<WorkViewModel>();
            builder.Services.AddTransient<WorkPage>();


            //联系人相关
            builder.Services.AddTransient<ContactFormViewModel>();
            builder.Services.AddTransient<ContactFormPage>();
            builder.Services.AddTransient<ContactPage>();
            builder.Services.AddTransient<ContactViewModel>();

            //标签相关
            builder.Services.AddTransient<TagFormViewModel>();
            builder.Services.AddTransient<TagFormPage>();
            builder.Services.AddTransient<TagViewModel>();
            builder.Services.AddTransient<TagPage>();

            //首页
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            return app;
        }
    }
}
