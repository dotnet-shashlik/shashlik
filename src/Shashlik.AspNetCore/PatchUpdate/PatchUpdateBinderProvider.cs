﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Guc.Utils.Extensions;
using System.Threading.Tasks;
using Guc.Utils.PatchUpdate;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Guc.AspNetCore.PatchUpdate
{
    public class PatchUpdateBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.BindingInfo.BindingSource != BindingSource.Body)
                return null;
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Metadata.ModelType.IsChildTypeOf<PatchUpdateBase>())
                return new BinderTypeModelBinder(typeof(PatchUpdateBinder));

            return null;
        }
    }
}
