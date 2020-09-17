using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Shashlik.Utils.Extensions;
using System.Threading.Tasks;

namespace Shashlik.AspNetCore.PatchUpdate
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
