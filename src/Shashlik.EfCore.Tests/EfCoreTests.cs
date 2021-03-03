using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.EfCore.Tests.Entities;
using Shashlik.Kernel;
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
        public async Task CustomTransactionBeginFunctionTest()
        {
            var name = Guid.NewGuid().ToString();
            var role = "add_user_test_role";
            var roles = new[] {role};

            var testManager = GetService<TestManager>();
            await using var tran = DbContext.BeginNestedTransactionByAsync();

            try
            {
                await testManager.CreateUserWithEfNestTransaction(name, roles, false);

                await tran.CommitAsync();
            }
            catch (Exception)
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
        public void NestedTransactionSyncLogicTest()
        {
            using var tran = DbContext.BeginNestedTransaction();
            DbContext.GetCurrentNestedTransaction().ShouldBe(tran);

            AsyncMethod(tran).GetAwaiter().GetResult();
            SyncMethod(tran);
        }

        [Fact]
        public async Task NestedTransactionAsyncLogicTest()
        {
            await using var tran = DbContext.BeginNestedTransactionByAsync();
            DbContext.GetCurrentNestedTransaction().ShouldBe(tran);

            await AsyncMethod(tran);
            SyncMethod(tran);
        }

        private async Task AsyncMethod(IDbContextTransaction topTran)
        {
            await using var tran2 = DbContext.BeginNestedTransactionByAsync();
            DbContext.GetCurrentNestedTransaction().ShouldBe(topTran);
        }

        private void SyncMethod(IDbContextTransaction topTran)
        {
            using var tran2 = DbContext.BeginNestedTransaction();
            DbContext.GetCurrentNestedTransaction().ShouldBe(topTran);
        }

        [Fact]
        public void TransactionForeachCommitSyncTest()
        {
            var role = "add_user_test_role";
            var roles = new[] {role};

            var testManager = GetService<TestManager>();
            var names = new List<string>();
            for (int i = 0; i < new Random().Next(5, 10); i++)
            {
                using var tran = DbContext.BeginNestedTransaction();
                DbContext.GetCurrentNestedTransaction().ShouldBe(tran);

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
                catch (Exception)
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
            var names = new List<string>();

            for (int i = 0; i < new Random().Next(5, 10); i++)
            {
                using var tran = DbContext.BeginNestedTransaction();
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
                catch (Exception)
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
            var names = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                await using var tran = DbContext.BeginNestedTransactionByAsync();
                try
                {
                    for (int j = 0; j < 1; j++)
                    {
                        var name = Guid.NewGuid().ToString();
                        names.Add(name);
                        await testManager.CreateUserWithEfNestTransaction(name, roles, false);
                    }

                    await tran.CommitAsync();
                }
                catch (Exception)
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
            var names = new List<string>();
            for (int i = 0; i < new Random().Next(5, 10); i++)
            {
                await using var tran = DbContext.BeginNestedTransactionByAsync();
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
                catch (Exception)
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
            await using var tran = DbContext.BeginNestedTransaction();


            try
            {
                await testManager.CreateUserWithEfNestTransaction(name, roles, false);

                await tran.CommitAsync();
            }
            catch (Exception)
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
                ex.Message.ShouldBe("事务应该回滚");
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

        [Fact]
        public async Task MigrationTests()
        {
            var serviceProvider = GetService<IServiceProvider>();
            var kernelServices = GetService<IKernelServices>();
            var @lock = GetService<ILock>();

            await serviceProvider.MigrationAsync(typeof(TestDbContext6));
            await serviceProvider.MigrationAsync<TestDbContext3>();
            await kernelServices.Services.MigrationAsync<TestDbContext6>(@lock);

            // 所有的都迁移完成了，可以操作数据了

            var dbContext1 = GetService<TestDbContext1>();
            var dbContext2 = GetService<TestDbContext2>();
            var dbContext3 = GetService<TestDbContext3>();
            var dbContext4 = GetService<TestDbContext4>();
            var dbContext5 = GetService<TestDbContext5>();
            var dbContext6 = GetService<TestDbContext6>();

            dbContext1.Set<Users>().ToList().ShouldNotBeNull();
            dbContext2.Set<Users>().ToList().ShouldNotBeNull();
            dbContext3.Set<Users>().ToList().ShouldNotBeNull();
            dbContext4.Set<Users>().ToList().ShouldNotBeNull();
            dbContext5.Set<Users>().ToList().ShouldNotBeNull();
            dbContext6.Set<Users>().ToList().ShouldNotBeNull();
        }

        [Fact]
        public void GetAllEntityTypesTest()
        {
            var dbContext1 = GetService<TestDbContext1>();
            dbContext1.GetAllEntityTypes().ShouldContain(typeof(Roles));
            dbContext1.GetAllEntityTypes().ShouldContain(typeof(Users));
        }
    }
}