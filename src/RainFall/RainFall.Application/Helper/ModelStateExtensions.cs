using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace RainFall.Application.Helper;

public static class ModelStateExtensions
{
    public static string Errors(this ModelStateDictionary modelState)
    {
        var errors = modelState.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.Exception?.Message ?? x.ErrorMessage);

        return JsonConvert.SerializeObject(errors);
    }
}