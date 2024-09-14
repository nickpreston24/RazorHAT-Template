using MyRazorHatApp;
using MyRazorHatApp.Extensions;
using Neo4j.Driver;
using DotEnv.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmbeddedResourceQuery, EmbeddedResourceQuery>();

new EnvLoader().Load();

// This method gets called by the runtime. Use this method to add services to the container.
void ConfigureServices(IServiceCollection services)
{
    var reader = new EnvReader();
    string uri = reader["NEO4J_URI"] ?? string.Empty;
    string user = reader["NEO4J_USER"] ?? string.Empty;
    string password = reader["NEO4J_PASSWORD"] ?? string.Empty;

    services.AddControllers();
    services.AddSingleton(GraphDatabase.Driver(
        uri
        , AuthTokens.Basic(
            user,
            password
        )
    ));
    
    services.AddSingleton<IAirtableRepo, AirtableRepo>();
}

ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
