# Splunk extended metrics (using statsd)

[![Build Status](https://dev.azure.com/raj0510/Splunk.Metrics/_apis/build/status/splunk.metrics?branchName=master)](https://dev.azure.com/raj0510/Splunk.Metrics/_build/latest?definitionId=2&branchName=master)

## FAQ

* Why did you create yet another statsd client especially for Splunk?

Splunk supports [expanded statsd metrics](https://docs.splunk.com/Documentation/SplunkCloud/latest/Metrics/GetMetricsInStatsd). This library supports dimensions and sample rates.

## Definging your statsd metric bucket namespace

The statsd bucket consists of 6 parts

* The first and second are the feature and action eg order-processor.orders, ingestion.entities
* The sixth is the event (and should be defined in the past as a fact) eg succeeded, failed, processed, etc

The following are examples of the full statsd string generated if you follow this guideline:

```js
order-processor.orders.processed
ingestion.entities.handled
```

## How to add metrics to your code

* Install the Nuget package **Splunk.Metrics.Abstractions**
```powershell
Install-Package Splunk.Metrics.Abstractions
```

* Inject **IStatsPublisher** from the installed library and call the methods for counts, gauges and timing

```csharp
public class Foo
{
   Foo(IStatsPublisher statsPublisher)
   {
      this.statsPublisher = statsPublisher;
   }

   public async Task DoSomething()
   {
      await statsPublisher.IncrementAsync("order-processor.orders.processed");
   }
}
```

## Bootstrapping .NET Core Applications
* Install the Nuget package **Splunk.Metrics.Statsd**

```powershell
Install-Package Splunk.Metrics.Statsd
```
* In your startup or bootstrapper:

```csharp
serviceCollection.AddTransient<IStatsPublisher, StatsPublisher>();
serviceCollection.Configure<MetricsConfiguration>(configuration.GetSection("Stats"));
```

* In your appsettings file, define the namespace

```json
"Stats": {
    "Prefix": "some-product.some-service" 
}
```

## Bootstrapping Legacy Full .NET Applications
* Install the Nuget package **Splunk.Metrics.Statsd**

```powershell
Install-Package Splunk.Metrics.Statsd
```
* In your startup or bootstrapper:

```csharp
container.RegisterType<IStatsPublisher, StatsPublisher>();
container.RegisterInstance(new StatsConfiguration(...)));
```

* In your appsettings file, define the namespace

```json
"Stats": {
    "Prefix": "some-product.some-service"
}
```

## Adding HTTP Metrics (ASP.NET Core)
* Install the Nuget package **Splunk.Metrics.Http**
```powershell
Install-Package Splunk.Metrics.Http
```

* In your startup, add the HTTP metrics middleware to the OWIN pipeline. Note that this should be one of the first middlewares in the pipeline to effectively capture metrics for all requests:

```csharp
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHttpMetrics();
            app.UseMvc();
        }
```

You can optionally add dimensions to your http metrics from your controller without accessing the stats publisher as follows:

```csharp
Request.HttpContext.SetDimensionForHttpMetrics("some-dimension", "some-dimension-value");
```

## Adding HTTP Metrics (Web API 2.x)
* Install the Nuget package **Splunk.Metrics.WebApi**
```powershell
Install-Package Splunk.Metrics.WebApi
```

* In your startup, add the filter `HttpMetricsFilter` to the Http Configuration and the HTTP metrics middleware to the OWIN pipeline. Note that this should be one of the first middlewares in the pipeline to effectively capture metrics for all requests:

```csharp
\\ Startup.cs
public void Configuration(IAppBuilder app)
{
    var httpConfiguration = new HttpConfiguration();
    httpConfiguration.Filters.Add(new HttpMetricsFilter());
	
	\\ Additional config...

    app.UseHttpMetrics(_statsConfiguration)
        .UseWebApi(httpConfiguration);
}
```

You can optionally add dimensions to your http metrics from your controller without accessing the stats publisher as follows:

```csharp
Request.GetOwinContext().SetDimensionForHttpMetrics("some-dimension", some-dimension-value);
```
