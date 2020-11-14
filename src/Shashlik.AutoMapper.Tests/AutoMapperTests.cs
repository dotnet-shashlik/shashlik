using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Shashlik.Kernel;
using Shashlik.Kernel.Test;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.AutoMapper.Tests
{
    public class AutoMapperTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AutoMapperTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Tests()
        {
            ShashlikAutoMapper.Instance.ShouldNotBeNull();

            {
                var userEntity = new UserEntity
                {
                    Id = 1,
                    Name = "zhang san",
                    BirthDay = new DateTime(1990, 1, 1)
                };

                var dto1 = userEntity.MapTo<UserDto1>();
                dto1.Id.ShouldBe(userEntity.Id);
                dto1.Name.ShouldBe(userEntity.Name);
            }

            {
                var userEntity = new UserEntity
                {
                    Id = 1,
                    Name = "zhang san",
                    BirthDay = new DateTime(1990, 1, 1)
                };

                var dto1 = userEntity.MapTo<UserDto2>();
                dto1.BirthDay.ShouldBe(userEntity.BirthDay);
                dto1.Name.ShouldBe($"Hello {userEntity.Name}");
            }

            {
                var userDto = new UserDto3()
                {
                    Id = 1,
                    Name = "zhang san",
                };

                var userEntity = userDto.MapTo<UserEntity>();
                userEntity.BirthDay.ShouldBe(default);
                userEntity.Name.ShouldBe(userDto.Name);
                userEntity.Id.ShouldBe(userDto.Id);
            }

            {
                var userDto = new UserDto4()
                {
                    Name = "Hello zhang san",
                    BirthDay = new DateTime(1990, 1, 1)
                };

                var userEntity = userDto.MapTo<UserEntity>();
                userEntity.BirthDay.ShouldBe(userDto.BirthDay);
                userEntity.Name.ShouldBe("zhang san");
            }

            {
                var users = new List<UserEntity>
                {
                    new UserEntity
                    {
                        Id = 1,
                        Name = "zhang san",
                        BirthDay = new DateTime(1990, 1, 1)
                    },
                    new UserEntity
                    {
                        Id = 2,
                        Name = "li si",
                        BirthDay = new DateTime(1990, 1, 1)
                    }
                };

                var dtoList = users.AsQueryable()
                    .QueryTo<UserDto1>()
                    .ToList();
                dtoList.Count.ShouldBe(2);
                dtoList[0].Id.ShouldBe(users[0].Id);
                dtoList[1].Id.ShouldBe(users[1].Id);
            }

            {
                var userEntity = new UserEntity
                {
                    Id = 1,
                    Name = "zhang san",
                    BirthDay = new DateTime(1990, 1, 1)
                };

                var userDto = new UserDto1
                {
                    Id = 2,
                    Name = "li si"
                };

                userEntity.MapTo(userDto);
                userDto.Id.ShouldBe(userEntity.Id);
                userDto.Name.ShouldBe(userEntity.Name);
            }
        }
    }

    public class UserEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime BirthDay { get; set; }
    }

    public class UserDto1 : IMapFrom<UserEntity>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class UserDto2 : IMapFrom<UserEntity, UserDto2>
    {
        public string Name { get; set; }

        public DateTime BirthDay { get; set; }

        public void Config(IMappingExpression<UserEntity, UserDto2> mapper)
        {
            mapper.ForMember(
                r => r.Name,
                r => r.MapFrom(f => $"Hello {f.Name}"));
        }
    }

    public class UserDto3 : IMapTo<UserEntity>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class UserDto4 : IMapTo<UserEntity, UserDto4>
    {
        public string Name { get; set; }

        public DateTime BirthDay { get; set; }


        public void Config(IMappingExpression<UserDto4, UserEntity> mapper)
        {
            mapper.ForMember(
                r => r.Name,
                r => r.MapFrom(f => f.Name.Replace("Hello", "").Trim())
            );
        }
    }
}