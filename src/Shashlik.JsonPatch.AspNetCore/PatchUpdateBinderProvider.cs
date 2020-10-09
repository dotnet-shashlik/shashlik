using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Shashlik.Utils.Extensions;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace Shashlik.JsonPatch.AspNetCore
{
    public class PatchUpdateBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.BindingInfo.BindingSource != BindingSource.Body)
                return null;
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Metadata.ModelType.IsSubTypeOf<PatchUpdateBase>())
                return new BinderTypeModelBinder(typeof(PatchUpdateBinder));

            return null;
        }
    }
}
