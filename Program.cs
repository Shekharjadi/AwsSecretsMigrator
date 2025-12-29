using Amazon.SecretsManager;
using Amazon.Extensions.NETCore.Setup;
using AwsSecretsMigrator.Services;

var builder = WebApplication.CreateBuilder(args);

// Register AWS Secrets Manager ONLY
builder.Services.AddAWSService<IAmazonSecretsManager>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// 🔹 REGISTER YOUR SERVICE 👇 (MISSING EARLIER)
builder.Services.AddScoped<AwsSecretsService>();



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
     pattern: "{controller=Secrets}/{action=Index}/{id?}");

app.Run();
