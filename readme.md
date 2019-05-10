# Axoom.Extensions.Prometheus.Standalone

[![NuGet package](https://img.shields.io/nuget/v/Axoom.Extensions.Prometheus.Standalone.svg)](https://www.nuget.org/packages/Axoom.Extensions.Prometheus.Standalone/)
[![Build status](https://img.shields.io/appveyor/ci/AXOOM/axoom-extensions-prometheus-standalone.svg)](https://ci.appveyor.com/project/AXOOM/axoom-extensions-prometheus-standalone)

This library provides a minimalist HTTP server for exposing Prometheus metrics on .NET Core without MVC routing. Extends the [Nexogen Prometheus library](https://github.com/nexogen-international/Nexogen.Libraries.Metrics).

It is intended as a replacement for the [Nexogen.Libraries.Metrics.Prometheus.Standalone](https://www.nuget.org/packages/Nexogen.Libraries.Metrics.Prometheus.Standalone) library. It uses [Microsoft.AspNetCore.Http](https://www.nuget.org/packages/Microsoft.AspNetCore.Http/) instead of [NETStandard.HttpListener](https://www.nuget.org/packages/NETStandard.HttpListener/) for better reliability.

# Usage

Add/use the Prometheus server by using the default configuration (Port: 5000).

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddOptions()
        .AddPrometheusServer();
}
```

Or override the default configuration by providing a configuration entry in the `appsetting.json`:

```json
{
    "PrometheusServer": {"Port": 5000}
}
```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddOptions()
        .AddPrometheusServer(Configuration.GetSection("PrometheusServer"));
}
```

## On Windows

You will need to run the following in an Admin PowerShell once to allow non-admin processes to bind to port 5000:

```powershell
netsh http add urlacl http://*:5000/ user=$env:USERDOMAIN\$env:USERNAME
```
