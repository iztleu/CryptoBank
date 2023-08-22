using System.Text;
using GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace GenerivHosting.Kestrel.Endpoints.Middlewares;

public class ExceptionPageMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        try
        {
            await next();
        }
        catch (Exception ex)
        {
            var htmlTemplate =
                @"
<!DOCTYPE html>
<html>
  <head>
    <title>HTTP 500 Internal Server Error</title>
  </head>
  <body>
    <p>Internal Server Error occured. Exception:</p>
    <p>{0}</p>
  </body>
</html>
";
            
            responseFeature.Headers.Add("Content-Type", new StringValues("text/html; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(string.Format(htmlTemplate, ex)));
        }
    }
}