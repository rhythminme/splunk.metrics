## Adding HTTP Metrics
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