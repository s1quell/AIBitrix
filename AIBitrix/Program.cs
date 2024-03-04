using AIBitrix.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Во время разработки использовать сервер с портами 5005
#if DEBUG
    serverOptions.ListenAnyIP(5260, options => options.UseHttps());
#endif
                
    // Во время деплоя на сервер использовать стандартные порты HTTP протокола
#if (!DEBUG)
    serverOptions.ListenAnyIP(443,
        portOptions => { portOptions.UseHttps(h => { h.UseLettuceEncrypt(serverOptions.ApplicationServices); }); }
    );
    serverOptions.ListenAnyIP(80);
#endif
});

#if (!DEBUG)
            builder.Services.AddLettuceEncrypt();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    AIController.Init();
    var controller = new TelegramController("6611949018:AAEhAttS_Q3Jmf10rRZFMOLiGyGrIImSkQQ", app.Logger);
}
catch (Exception e)
{
    app.Logger.LogCritical(e.Message);
}

app.Run();