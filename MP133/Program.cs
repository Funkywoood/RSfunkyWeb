using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MP133;
using MP133.Data;
using MP133.Model;

var builder = WebApplication.CreateBuilder(args);

var kvUri = "https://rstresor.vault.azure.net/"; 
builder.Services.AddSingleton(new SecretClient(new Uri(kvUri), new DefaultAzureCredential()));

builder.Services.AddRazorPages();


builder.Services.AddTransient<EmailService>(provider =>
{
    var secretClient = provider.GetRequiredService<SecretClient>();
    var logger = provider.GetRequiredService<ILogger<EmailService>>();
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new EmailService(builder.Configuration, secretClient, logger, httpClient);
});

builder.Services.AddHttpClient();

builder.Services.AddLogging();

builder.Services.Configure<ReCaptchaSettings>(builder.Configuration.GetSection("reCAPTCHA"));

builder.Services.AddDbContext<MP133Context>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AzureConnection");

    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
        sqlOptions.CommandTimeout(30); 
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
    await emailService.InitializeSecretsAsync();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    app.MapRazorPages();

    app.Run();
}
