﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pusula.Training.HealthCare.Blazor.Services;
using Pusula.Training.HealthCare.Blazor.Services.Treatment;
using Pusula.Training.HealthCare.ProtocolTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Syncfusion.Blazor;

namespace Pusula.Training.HealthCare.Blazor;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]!))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                    IndexFormat = "Pusula-Training-HealthCare-log-{0:yyyy.MM}"
                })
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            
            //  protocol state service
            builder.Services.AddSingleton<ProtocolStateService>();

            // Statik varlıklar için
            builder.WebHost.UseStaticWebAssets();

            // Syncfusion için lisans kaydı
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(configuration["SyncFusion:Key"]);

            // Syncfusion Blazor hizmeti ekleniyor
            builder.Services.AddSyncfusionBlazor();

            //Custom adaptors for using on SFGrids
            builder.Services.AddScoped<AppointmentTypeAdaptor>();
            builder.Services.AddScoped<AppointmentStatisticsAdaptor>();
            builder.Services.AddScoped<AppointmentAdaptor>();
            builder.Services.AddScoped<DoctorLeaveAdaptor>();
            builder.Services.AddScoped<IcdAdaptor>();
            builder.Services.AddScoped<IcdReportAdaptor>();
            builder.Services.AddScoped<DoctorAdaptor>();
            builder.Services.AddScoped<MedicalStaffAdaptor>();
            builder.Services.AddScoped<ProtocolAdaptor>();
            builder.Services.AddScoped<ProtocolTypeAdaptor>();
            builder.Services.AddScoped<ProtocolStateService>();
            builder.Services.AddScoped<ProtocolDepartmentStatisticsAdaptor>();
            builder.Services.AddScoped<ProtocolPatientDepartmentListAdaptor>();
            builder.Services.AddScoped<ProtocolPatientDoctorListAdaptor>();
            builder.Services.AddScoped<ProtocolDoctorStatisticsAdaptor>();

            // Serilog ve diğer ayarları ekleme
            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();

            // Modül yapılandırması
            await builder.AddApplicationAsync<HealthCareBlazorModule>();
            var app = builder.Build();

            // Uygulama başlatma ve çalıştırma
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
