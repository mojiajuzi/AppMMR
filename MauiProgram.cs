using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using AppMMR.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using AppMMR.Pages;
using AppMMR.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppMMR
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
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
