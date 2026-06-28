using McHelper.Components;
using McHelper.Extensions;
using McHelper.Logic;

var builder = WebApplication.CreateBuilder(args);

//config
var secrets = builder.Configuration.GetSection("App").Get<Config>() ?? throw new YouIdiotException("Secrets could not be loaded!");
builder.Services.AddSingleton(secrets);
builder.Services.AddSingleton(secrets.Server);
builder.Services.AddSingleton(sp => new Paths(secrets, builder.Environment.ContentRootPath));

//services
builder.Services.AddSingleton<ModsRepo>();
builder.Services.AddSingleton<ModService>();
builder.Services.AddSingleton<ServerControl>();

//razor
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
