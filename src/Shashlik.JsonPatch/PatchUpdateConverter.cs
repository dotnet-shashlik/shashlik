using System;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.JsonPatch
{
    public class PatchUpdateConverter
    {
        public PatchUpdateConverter(string sourcePro, Func<object, (string targetPro, object targetValue)> convert)
        {
            if (string.IsNullOrWhiteSpace(sourcePro))
                throw new ArgumentException("sourcePro can not be empty.", nameof(sourcePro));

            SourcePro = sourcePro;
            Convert = convert ?? throw new ArgumentNullException(nameof(convert));
        }

        public string SourcePro { get; set; }

        public Func<object, (string targetPro, object targetValue)> Convert { get; set; }
    }
}