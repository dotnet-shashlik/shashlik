using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.PatchUpdate;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Shashlik.AspNetCore.PatchUpdate
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
