using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Management_Dashboard.Filters
{
    public class BasePathDocumentFilter : IDocumentFilter
    {
        private readonly string _basePath;

        public BasePathDocumentFilter(string basePath)
        {
            _basePath = basePath;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var newPaths = new OpenApiPaths();

            foreach (var path in swaggerDoc.Paths)
            {
                newPaths.Add("/"  + _basePath + path.Key, path.Value);
            }
            swaggerDoc.Paths = newPaths;
        }
    }
}
