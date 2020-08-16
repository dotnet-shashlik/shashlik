using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Shashlik.Utils.Extensions;
using System.Threading.Tasks;
using Shashlik.Utils.PatchUpdate;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Shashlik.AspNetCore.PatchUpdate
{
    public class PatchUpdateBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var str = bindingContext.HttpContext.Request.BodyReader.AsStream().ReadToString();
            var jobject = JsonConvert.DeserializeObject<JObject>(str);
            var instance = Activator.CreateInstance(bindingContext.ModelType, jobject);
            bindingContext.Result = ModelBindingResult.Success(instance);
            return Task.CompletedTask;
        }
    }
}
