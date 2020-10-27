using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Shashlik.EfCore.Tests;
using Shashlik.EfCore.Tests.Entities;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Captcha.Tests
{
    public class EfCoreTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EfCoreTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }

        private TestDbContext DbContext => GetService<TestDbContext>();

        [Fact]
        public async Task TransactionalSuccessTest()
        {
            var testManager = GetService<TestManager>();
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};
            await testManager.CreateUser(name, roles, false);

            var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
            userEntity.ShouldNotBeNull();
            var roleEntity = DbContext.Set<Roles>().FirstOrDefault(r => r.UserId == userEntity.Id && r.Name == role);
            roleEntity.ShouldNotBeNull();
        }

        [Fact]
        public async Task TransactionalFailTest()
        {
            var testManager = GetService<TestManager>();
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};
            try
            {
                await testManager.CreateUser(name, roles, true);
            }
            catch (Exception ex)
            {
                ex.InnerException!.Message.ShouldBe("事务应该回滚");
            }

            var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
            userEntity.ShouldBeNull();
            var roleEntity = DbContext.Set<Roles>().FirstOrDefault(r => r.Name == role && r.User.Name == name);
            roleEntity.ShouldBeNull();
        }

        [Fact]
        public async Task SoftDeleteTest()
        {
            var testManager = GetService<TestManager>();
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};
            await testManager.CreateUser(name, roles, false);

            {
                var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
                userEntity.ShouldNotBeNull();
                var roleEntity = DbContext.Set<Roles>()
                    .FirstOrDefault(r => r.UserId == userEntity.Id && r.Name == role);
                roleEntity.ShouldNotBeNull();

                userEntity.IsDeleted = true;
                userEntity.DeleteTime = DateTime.Now;

                DbContext.SaveChanges();
            }

            {
                var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
                userEntity.ShouldBeNull();
                var roleEntity = DbContext.Set<Roles>()
                    .FirstOrDefault(r => r.Name == role && r.User.Name == name);
                roleEntity.ShouldBeNull();
            }
        }

        [Fact]
        public async Task PerformanceTest()
        {
            var testManager = GetService<TestManager>();
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};
            var count = 1000;

            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < count; i++)
                {
                    await testManager.CreateUserWithEfTransaction(name, roles, false);
                }

                stopwatch.Stop();
                _testOutputHelper.WriteLine($"Ef原生事务执行{count}次,总计耗时:{stopwatch.ElapsedMilliseconds / 1000M}秒");
            }

            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < count; i++)
                {
                    await testManager.CreateUserWithEfNestTransaction(name, roles, false);
                }

                stopwatch.Stop();
                _testOutputHelper.WriteLine($"Ef嵌套事务执行{count}次,总计耗时:{stopwatch.ElapsedMilliseconds / 1000M}秒");
            }

            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < count; i++)
                {
                    await testManager.CreateUser(name, roles, false);
                }

                stopwatch.Stop();
                _testOutputHelper.WriteLine(
                    $"Transactional特性事务执行{count}次,总计耗时:{stopwatch.ElapsedMilliseconds / 1000M}秒");
            }
        }
    }
}