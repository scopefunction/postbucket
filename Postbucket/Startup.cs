using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Postbucket.BLL;
using Postbucket.Services;

namespace Postbucket;


public static class EnvironmentConstants
{
    public static string Cosmos = "POSTBUCKET_COSMOS_DB_KEY";
    public static string Blob = "POSTBUCKET_BLOB_STORAGE_KEY";
    public static string SendGrid = "POSTBUCKET_SENDGRID_KEY";
}

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
        });

        // services.AddSingleton(_ => new CosmosClient(Configuration["Cosmos:ConnectionString"]
        //     .Replace("{COSMOS_DB_KEY}",
        //         Environment.GetEnvironmentVariable(EnvironmentConstants.Cosmos))));

        services.AddTransient<IDocumentService, DocumentService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddSingleton<AppSettings, AppSettings>(_ => new AppSettings
        {
            SendGridAccessKey = Environment.GetEnvironmentVariable(EnvironmentConstants.SendGrid) ?? string.Empty,
            BlobStorageConnectionString = Configuration["Blob:ConnectionString"],
            CosmosConnectionString = Configuration["Cosmos:ConnectionString"],
            FromEmail = Configuration["Sender:Email"]
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // app.UseHsts();
        }

        // app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("AllowAll");

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllerRoute(
                "default",
                "{controller=Default}/{action=Default}/{id?}");
        });
    }
}