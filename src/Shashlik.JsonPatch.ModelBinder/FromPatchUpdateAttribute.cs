using Microsoft.AspNetCore.Mvc;

namespace Shashlik.JsonPatch.ModelBinder
{
    public class FromPatchUpdateAttribute : ModelBinderAttribute
    {
        public FromPatchUpdateAttribute() : base(typeof(PatchUpdateBinder))
        {
        }
    }
}