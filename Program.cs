using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AwsSecretsMigrator.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// 1️⃣ Setup AWS Secrets Manager Client
// --------------------
var secretsClient = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.EUNorth1); // region matches your secret

// --------------------
// 2️⃣ Load the combined secret (DB + AppSettings)
// --------------------
try
{
    var secretResponse = await secretsClient.GetSecretValueAsync(new GetSecretValueRequest
    {
        SecretId = "sample/dev/secretes/" // exact secret name from AWS
    });

    if (!string.IsNullOrEmpty(secretResponse.SecretString))
    {
        // Deserialize JSON into dictionary
        var secretDict = JsonSerializer.Deserialize<Dictionary<string, string>>(secretResponse.SecretString);

        if (secretDict != null)
        {
            // Set DB connection string
            if (secretDict.TryGetValue("AwsSecreteConnection", out var dbConn))
                builder.Configuration["ConnectionStrings:DefaultConnection"] = dbConn;

            // Load AppSettings
            foreach (var kvp in secretDict)
            {
                if (kvp.Key != "AwsSecreteConnection") // skip DB string
                    builder.Configuration[$"AppSettings:{kvp.Key}"] = kvp.Value;
            }
        }
    }
    else
    {
        throw new Exception("SecretString is empty.");
    }
}
catch (ResourceNotFoundException)
{
    Console.WriteLine("ERROR: The secret 'sample/dev/secretes/' was not found in AWS Secrets Manager.");
    throw;
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: Failed to load secret from AWS. {ex.Message}");
    throw;
}

// --------------------
// 3️⃣ EF Core DbContext
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// 4️⃣ MVC & Services
// --------------------
builder.Services.AddControllersWithViews();
// 1️⃣ Register AWS Secrets Manager client in DI
builder.Services.AddAWSService<IAmazonSecretsManager>();

// 2️⃣ Register your custom service
builder.Services.AddScoped<AwsSecretsService>();


var app = builder.Build();

// --------------------
// 5️⃣ Middleware
// --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// --------------------
// 6️⃣ Routes
// --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employees}/{action=Index}/{id?}");

app.Run();
