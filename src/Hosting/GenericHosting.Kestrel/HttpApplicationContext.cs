using Microsoft.AspNetCore.Http.Features;

namespace GenericHosting.Kestrel;

internal record HttpApplicationContext(IFeatureCollection Features);