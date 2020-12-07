using System;
using System.Collections.Generic;
using System.Linq;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class CollectionTests
    {
        private class TestClass
        {
            public string Str { get; set; }
        }
        
        private class TestComparer : IEqualityComparer<TestClass>
        {
            public bool Equals(TestClass x, TestClass y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Str == y.Str;
            }

            public int GetHashCode(TestClass obj)
            {
                return obj.Str.GetHashCode();
            }
        }

        [Fact]
        public void NullOrEmptyTest()
        {
            new List<string>().IsNullOrEmpty().ShouldBeTrue();
            new Dictionary<string, string>().IsNullOrEmpty().ShouldBeTrue();
            new List<string>{"str1"}.IsNullOrEmpty().ShouldBeFalse();
            new HashSet<int>{1, 2}.IsNullOrEmpty().ShouldBeFalse();
        }

        [Fact]
        public void ForeachTest()
        {
            var list = new List<string>
                {
                    "str1",
                    "str2"
                };
            list.ForEachItem(s =>
            {
                list.Contains(s).ShouldBeTrue();
            });
            Should.Throw<Exception>(() => list.ForEachItem(null));
        }

        [Fact]
        public void MergeTest()
        {
            var from = new Dictionary<string, string>()
            {
                {"k1", "v1"},
                {"k2", "v2"},
            };
            
            var to = new Dictionary<string, string>()
            {
                {"k1", "v11"},
                {"k3", "v3"},
            };
            
            to.ContainsKey("k1").ShouldBeTrue();
            from.ContainsKey("k1").ShouldBeTrue();
            to["k1"].ShouldBe("v11");
            
            to.Merge(from);
            
            to.ContainsKey("k2").ShouldBeTrue();
            to["k2"].ShouldBe(from["k2"]);
            to["k1"].ShouldBe(from["k1"]);
            
            to.Merge(null);
            
            to.Count.ShouldBe(3);
        }

        [Fact]
        public void GetOrDefaultTest()
        {
            var dic = new Dictionary<string, string>()
            {
                {"k1", "v1"},
                {"k2", "v2"},
            };
            
            dic.GetOrDefault("k1").ShouldBe(dic["k1"]);
            dic.GetOrDefault("empty").ShouldBe(default);
            dic.GetOrDefault(null).ShouldBe(default);
        }

        [Fact]
        public void GetIndexTest()
        {
            var list = new List<string> {"str1", "str2"};
            list.GetIndex("str2").ShouldBe(1);
            list.GetIndex("str3").ShouldBe(-1);
        }

        [Fact]
        public void JoinTest()
        {
            var list = new List<string> {"str1", "str2"};
            Should.Throw<Exception>(() => list.Join(null));
            list.Join(",").ShouldBe(string.Join(",", list));
        }

        [Fact]
        public void ToReadonlyTest()
        {
            List<string> list = null;
            Should.Throw<Exception>(() => list.ToReadOnly());
            list = new List<string>(){"str1"};
            var readOnlyList = list.ToReadOnly();
            readOnlyList.Count.ShouldBe(list.Count);
        }

        [Fact]
        public void HasRepeatTests()
        {
            var list = new List<string> {"str1", "str1", "str2"};
            list.HasRepeat().ShouldBeTrue();
            var objList = new List<TestClass>
            {
                new TestClass() {Str = "str"},
                new TestClass() {Str = "str"},
                new TestClass() {Str = "str1"},
            };
            var objDic = new Dictionary<string, TestClass>()
            {
                {"k1", new TestClass() {Str = "str"}},
                {"k2", new TestClass() {Str = "str"}},
                {"k3", new TestClass() {Str = "str1"}},
            };
            objList.HasRepeat(new TestComparer()).ShouldBeTrue();

            objList.HasRepeat(t => t.Str).ShouldBeTrue();
            
            objDic.HasRepeat(x => x.Value, new TestComparer()).ShouldBeTrue();

            Should.Throw<Exception>(() => objDic.HasRepeat(null, new TestComparer()));
            objList = null;
            objDic = null;
            Should.Throw<Exception>(() => objDic.HasRepeat(x => x.Value, new TestComparer()));
            Should.Throw<Exception>(() => objList.HasRepeat(new TestComparer()));
        }

        [Fact]
        public void WhereIfTests()
        {
            var list = new List<string> {"str1", "str1", "str2"};
            var list1 = list.AsQueryable().WhereIf(true, s => s.EndsWith("2")).WhereIf(false, s => s.StartsWith("str")).ToList();
            list1.Count.ShouldBe(1);
            var list2 = list.WhereIf(true, s => s.EndsWith("2")).WhereIf(false, s => s.StartsWith("str")).ToList();
            list2.Count.ShouldBe(1);
            var query = list.AsQueryable();
            Should.Throw<Exception>(() => list.WhereIf(true, null));
            Should.Throw<Exception>(() => query.WhereIf(true, null));
            query = null;
            list = null;
            Should.Throw<Exception>(() => list.WhereIf(true, s => s.EndsWith("2")));
            Should.Throw<Exception>(() => query.WhereIf(true, s => s.EndsWith("2")));
        }

        [Fact]
        public void DoPageTests()
        {
            var list = new List<string>();
            for (var i = 0; i < 100; i++)
            {
                list.Add($"str{i}");
            }

            var query = list.AsQueryable();
            var page = list.DoPage(2, 10).ToList();
            page.Count.ShouldBe(10);
            page.First().ShouldBe("str10");
            var page2 = query.DoPage(3, 5).ToList();
            page2.Count.ShouldBe(5);
            page2.First().ShouldBe("str10");

            list = null;
            query = null;
            Should.Throw<Exception>(() => list.DoPage(1, 1));
            Should.Throw<Exception>(() => query.DoPage(1, 1));
        }

        [Fact]
        public void DataTableTests()
        {
            var objList = new List<TestClass>
            {
                new TestClass() {Str = "str"},
                new TestClass() {Str = "str"},
                new TestClass() {Str = "str1"},
                new TestClass() {Str = ""},
                new TestClass() {Str = " \r\t\n "},
            };
            var dataTable = objList.ToDataTable();
            dataTable.Columns.Count.ShouldBe(1);
            dataTable.Rows.Count.ShouldBe(objList.Count);
            dataTable.Rows[1].IsEmptyRow().ShouldBeFalse();
            dataTable.Rows[1].IsEmptyRow(false).ShouldBeFalse();
            dataTable.Rows[4].IsEmptyRow(false).ShouldBeFalse();
            dataTable.Rows[4].IsEmptyRow().ShouldBeTrue();
            dataTable.Rows[3].IsEmptyRow().ShouldBeTrue();
        }
        
    }
}