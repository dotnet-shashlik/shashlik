// using System.IO;
// using Microsoft.Extensions.Configuration;
// using Shashlik.Kernel.Test;
//
// namespace Shashlik.EfCore.Tests
// {
//     public class EfCoreTestConfigurationBuilder : ITestConfigurationBuilder
//     {
//         public void Build(IConfigurationBuilder builder)
//         {
//             var file = new FileInfo("./config.yaml").FullName;
//             builder.AddYamlFile(file);
//         }
//     }
// }