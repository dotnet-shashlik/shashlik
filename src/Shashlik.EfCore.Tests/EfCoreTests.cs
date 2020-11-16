using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Shashlik.EfCore.Tests.Entities;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.EfCore.Tests
{
    public class EfCoreTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EfCoreTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }

        private TestDbContext1 DbContext => GetService<TestDbContext1>();

        [Fact]
        public void TransactionForeachCommitSyncTest()
        {
            var role = "add_user_test_role";
            var roles = new[] {role};

            var testManager = GetService<TestManager>();
            var nestTran = GetService<IEfNestedTransaction<TestDbContext1>>();
            var names = new List<string>();
            for (int i = 0; i < new Random().Next(5, 10); i++)
            {
                using var tran = nestTran.Begin();
                try
                {
                    for (int j = 0; j < new Random().Next(5, 10); j++)
                    {
                        var name = Guid.NewGuid().ToString();
                        names.Add(name);
                        testManager.CreateUserWithEfNestTransaction(name, roles, false).GetAwaiter().GetResult();
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }
            }

            foreach (var name in names)
            {
                var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
                userEntity.ShouldNotBeNull();
                var roleEntity = DbContext.Set<Roles>()
                    .FirstOrDefault(r => r.UserId == userEntity.Id && r.Name == role);
                roleEntity.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TransactionForeachRollbackSyncTest()
        {
            var role = "add_user_test_role";
            var roles = new[] {role};

            var testManager = GetService<TestManager>();
            var nestTran = GetService<IEfNestedTransaction<TestDbContext1>>();
            var names = new List<string>();
            for (int i = 0; i < new Random().Next(5, 10); i++)
            {
                using var tran = nestTran.Begin();
                try
                {
                    for (int j = 0; j < new Random().Next(5, 10); j++)
                    {
                        var name = Guid.NewGuid().ToString();
                        names.Add(name);
                        testManager.CreateUserWithEfNestTransaction(name, roles, j % 2 == 0).GetAwaiter().GetResult();
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }
            }

            foreach (var name in names)
            {
                var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
                userEntity.ShouldBeNull();
                var roleEntity = DbContext.Set<Roles>().FirstOrDefault(r => r.Name == role && r.User.Name == name);
                roleEntity.ShouldBeNull();
            }
        }


        [Fact]
        public async Task TransactionForeachCommitAsyncTest()
        {
            var role = "add_user_test_role";
            var roles = new[] {role};

            var testManager = GetService<TestManager>();
            var nestTran = GetService<IEfNestedTransaction<TestDbContext1>>();
            var names = new List<string>();
            for (int i = 0; i < new Random().Next(5, 10); i++)
            {
                await using var tran = nestTran.Begin();
                try
                {
                    for (int j = 0; j < new Random().Next(5, 10); j++)
                    {
                        var name = Guid.NewGuid().ToString();
                        names.Add(name);
                        await testManager.CreateUserWithEfNestTransaction(name, roles, false);
                    }

                    await tran.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                }
            }

            foreach (var name in names)
            {
                var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
                userEntity.ShouldNotBeNull();
                var roleEntity = DbContext.Set<Roles>()
                    .FirstOrDefault(r => r.UserId == userEntity.Id && r.Name == role);
                roleEntity.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task TransactionForeachRollbackAsyncTest()
        {
            var role = "add_user_test_role";
            var roles = new[] {role};

            var testManager = GetService<TestManager>();
            var nestTran = GetService<IEfNestedTransaction<TestDbContext1>>();
            var names = new List<string>();
            for (int i = 0; i < new Random().Next(5, 10); i++)
            {
                await using var tran = nestTran.Begin();
                try
                {
                    for (int j = 0; j < new Random().Next(5, 10); j++)
                    {
                        var name = Guid.NewGuid().ToString();
                        names.Add(name);
                        await testManager.CreateUserWithEfNestTransaction(name, roles, j % 2 == 0);
                    }
                    await tran.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                }
            }

            foreach (var name in names)
            {
                var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
                userEntity.ShouldBeNull();
                var roleEntity = DbContext.Set<Roles>().FirstOrDefault(r => r.Name == role && r.User.Name == name);
                roleEntity.ShouldBeNull();
            }
        }


        [Fact]
        public async Task TransactionalSuccessTest()
        {
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};

            var testManager = GetService<TestManager>();

            var nestTran = GetService<IEfNestedTransaction<TestDbContext1>>();
            await using var tran = nestTran.Begin();

            try
            {
                await testManager.CreateUserWithEfNestTransaction(name, roles, false);

                await tran.CommitAsync();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw;
            }

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

                await DbContext.SaveChangesAsync();
            }

            {
                var userEntity = DbContext.Set<Users>().FirstOrDefault(r => r.Name == name);
                userEntity.ShouldBeNull();
                var roleEntity = DbContext.Set<Roles>()
                    .FirstOrDefault(r => r.Name == role && r.User.Name == name);
                roleEntity.ShouldBeNull();
            }
        }

        private static int count = 1000;

        [Fact]
        public async Task PerformanceAttributeTransactionalTest()
        {
            var testManager = GetService<TestManager>();
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};
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

        [Fact]
        public async Task PerformanceNestTransactionTest()
        {
            var testManager = GetService<TestManager>();
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                await testManager.CreateUserWithEfNestTransaction(name, roles, false);
            }

            stopwatch.Stop();
            _testOutputHelper.WriteLine($"Ef嵌套事务执行{count}次,总计耗时:{stopwatch.ElapsedMilliseconds / 1000M}秒");
        }

        [Fact]
        public async Task PerformanceOriginalTransactionTest()
        {
            var testManager = GetService<TestManager>();
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                await testManager.CreateUserWithEfTransaction(name, roles, false);
            }

            stopwatch.Stop();
            _testOutputHelper.WriteLine($"Ef原生事务执行{count}次,总计耗时:{stopwatch.ElapsedMilliseconds / 1000M}秒");
        }
    }
}