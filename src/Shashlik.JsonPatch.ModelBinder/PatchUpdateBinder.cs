using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Shashlik.Utils.Extensions;

namespace Shashlik.JsonPatch.ModelBinder
{
    public class PatchUpdateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var str = bindingContext.HttpContext.Request.BodyReader.AsStream().ReadToString();
            var jsonElement = JsonConvert.DeserializeObject<JsonElement>(str);
            var instance = Activator.CreateInstance(bindingContext.ModelType, jsonElement);
            bindingContext.Result = ModelBindingResult.Success(instance);
            return Task.CompletedTask;
        }
    }
}