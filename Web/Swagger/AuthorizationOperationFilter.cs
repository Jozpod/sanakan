using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Web.Swagger
{
    [ExcludeFromCodeCoverage]
    public class AuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var parameter = new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "The auth token",
            };

            operation.Parameters.Add(parameter);
        }
    }
}
