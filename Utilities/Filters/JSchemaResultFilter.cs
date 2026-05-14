using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Utilities.Responses;

namespace Utilities.Filters;

public class JSchemaResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? 200;
            string controllerName = context.RouteData.Values["controller"]?.ToString() ?? "";
            
            // Determinar el prefijo basado en el dominio (Mapping del TXT)
            string prefix = GetPrefixForController(controllerName);
            
            // Envolver la respuesta de manera profesional
            var wrapper = new ApiResponse<object>(
                statusCode,
                $"{prefix}{statusCode}",
                objectResult.Value
            );

            objectResult.Value = wrapper;
        }
        else if (context.Result is StatusCodeResult statusCodeResult)
        {
            var statusCode = statusCodeResult.StatusCode;
            string controllerName = context.RouteData.Values["controller"]?.ToString() ?? "";
            string prefix = GetPrefixForController(controllerName);

            context.Result = new ObjectResult(new ApiResponse<object>(
                statusCode,
                $"{prefix}{statusCode}",
                null
            )) { StatusCode = statusCode };
        }

        await next();
    }

    private string GetPrefixForController(string controllerName)
    {
        return controllerName switch
        {
            // MS 1: Identity & Admin
            "Auth" or "User" or "AdminCatalog" => "ID_",

            // MS 2: Student & Academic
            "Student" or "StudentArea" or "StudentSupport" or "Material" or "Assignment" or "Dialog" => "ST_",

            // MS 3: Clinical & Intelligence
            "TEA" or "CIE" or "PsychoeducationalAssessment" or "Canalization" or "Report" or "Notification" => "CL_",

            // Default
            _ => "GW_" // Gateway/General
        };
    }
}
