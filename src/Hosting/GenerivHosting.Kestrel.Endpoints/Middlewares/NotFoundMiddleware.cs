using System.Text;
using GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace GenerivHosting.Kestrel.Endpoints.Middlewares;

public class NotFoundMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;
        responseFeature.StatusCode = StatusCodes.Status404NotFound;
        
        var htmlTemplate =
            @"
<!DOCTYPE html>
<html>
  <head>
    <title>HTTP 404 Not Found</title>
  </head>
  <body>
    <p>HTTP 404 Not Found</p>
  </body>
</html>
";
        responseFeature.Headers.Add("Content-Type", new StringValues("text/html; charset=UTF-8"));
        await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(htmlTemplate));
    }
}