using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ConnectedOfficeAPI.Handler;
using ConnectedOfficeAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string kvURL = builder.Configuration.GetValue<string>("KeyVaultConfig:KVUrl");
string tenantID = builder.Configuration.GetValue<string>("KeyVaultConfig:TenantID");
string clientID = builder.Configuration.GetValue<string>("KeyVaultConfig:ClientID");
string clientSecret = builder.Configuration.GetValue<string>("KeyVaultConfig:ClientSecretID");

var credentials = new ClientSecretCredential(tenantID, clientID, clientSecret);
var client = new SecretClient(new Uri(kvURL), credentials);
builder.Configuration.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ConnectedOfficeContexts>(options =>
options.UseSqlServer(builder.Configuration.GetValue<string>("dbconn")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var titleBase = "Connected Office API";
    var description = "This is a Web API for Connected Office operations";
    var License = new OpenApiLicense()
    {
        Name = "MIT"
    };
    var Contact = new OpenApiContact()
    {
        Name = "Rechard Preston",
        Email = "Rechard.Preston@gmail.comm",
        Url = new Uri("https://soutiessandbox.webnode.page/")
    };
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
           new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             new string[] {}
        }
    });
    c.SwaggerDoc("v2.3", new OpenApiInfo
    {
        Version = "v2.3",
        Title = titleBase,
        Description = description,
        License = License,
        Contact = Contact
    });
});

var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");

builder.Services.AddScoped<IRefereshTokenGenerator, RefereshTokenGenerator>();

var _jwtsettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.Configure<JwtSettings>(_jwtsettings);

builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v2.3/swagger.json", "ConnectedOfficeAPI v2.3");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
