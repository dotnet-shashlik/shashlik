// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Shashlik.Kernel;
//
// namespace Shashlik.EfCore.Tests
// {
//     public class EfCoreTestAutowire : IServiceAssembler
//     {
//         public void Configure(IKernelServices kernelServices)
//         {
//             kernelServices.Services.AddDbContextPool<TestDbContext1>(r =>
//                 {
//                     var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Conn1");
//                     r.UseMySql(conn, ServerVersion.FromString("5.7"), db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
//                 }, 5)
//                 ;
//
//             kernelServices.Services.AddDbContextPool<TestDbContext2>(r =>
//                 {
//                     var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Conn2");
//                     r.UseMySql(conn, ServerVersion.FromString("5.7"), db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
//                 }, 5)
//                 ;
//
//             kernelServices.Services.AddDbContextPool<TestDbContext3>(r =>
//                 {
//                     var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Conn3");
//                     r.UseMySql(conn, ServerVersion.FromString("5.7"), db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
//                 }, 5)
//                 ;
//
//             kernelServices.Services.AddDbContextPool<TestDbContext4>(r =>
//                 {
//                     var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Conn4");
//                     r.UseMySql(conn, ServerVersion.FromString("5.7"), db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
//                 }, 5)
//                 ;
//
//             kernelServices.Services.AddDbContextPool<TestDbContext5>(r =>
//                 {
//                     var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Conn5");
//                     r.UseMySql(conn, ServerVersion.FromString("5.7"), db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
//                 }, 5)
//                 ;
//
//             kernelServices.Services.AddDbContextPool<TestDbContext6>(r =>
//                 {
//                     var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Conn6");
//                     r.UseMySql(conn, ServerVersion.FromString("5.7"), db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
//                 }, 5)
//                 ;
//
//
//             kernelServices.Services.AddAutoMigration<TestDbContext4>();
//             kernelServices.Services.AddAutoMigration(typeof(TestDbContext5));
//         }
//     }
// }