using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shashlik.Utils.Extensions;

namespace Shashlik.JsonPatch.AspNetCore
{
    public class PatchUpdateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var str = bindingContext.HttpContext.Request.BodyReader.AsStream().ReadToString();
            var jObject = JsonConvert.DeserializeObject<JObject>(str);
            var instance = Activator.CreateInstance(bindingContext.ModelType, jObject);
            bindingContext.Result = ModelBindingResult.Success(instance);
            return Task.CompletedTask;
        }
    }
}