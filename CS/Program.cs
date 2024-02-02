using DevExpress.DataAccess.Sql;
using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using ASPNETCoreDashboardCustomPanel;
using DevExpress.DataAccess.Json;
using ASPNETCoreDashboardCustomPanel.Code;

var builder = WebApplication.CreateBuilder(args);

IFileProvider? fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration? configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
    .AddEntityFrameworkSqlite()
    .AddDbContext<NorthwindContext>(options => options
        .UseSqlite("Data Source=App_Data/nwind.db")
    );

builder.Services.AddDevExpressControls();
builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
    DashboardConfigurator configurator = new DashboardConfigurator();

    configurator.SetDashboardStorage(
        new CustomDashboardStorage(fileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath,
        serviceProvider.GetService<NorthwindContext>()!));

    DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
    DashboardJsonDataSource jsonDataSourceSupport = new DashboardJsonDataSource("Support");
    jsonDataSourceSupport.ConnectionName = "jsonSupport";
    jsonDataSourceSupport.RootElement = "Employee";
    dataSourceStorage.RegisterDataSource("jsonDataSourceSupport", jsonDataSourceSupport.SaveToXml());
    configurator.SetDataSourceStorage(dataSourceStorage);

    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
    configurator.ConfigureDataConnection += (s, e) => {
        if (e.ConnectionName == "jsonSupport") {
            Uri fileUri = new Uri(fileProvider.GetFileInfo("App_Data/Support.json").PhysicalPath, UriKind.RelativeOrAbsolute);
            JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
            jsonParams.JsonSource = new UriJsonSource(fileUri);
            e.ConnectionParameters = jsonParams;
        }
    };

    return configurator;
});

builder.Services.AddDevExpressControls(settings => settings.Resources = ResourcesType.None);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseDevExpressControls();
EndpointRouteBuilderExtension.MapDashboardRoute(app, "api/dashboard", "DefaultDashboard");

app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
