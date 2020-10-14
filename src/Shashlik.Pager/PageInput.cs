using System.ComponentModel.DataAnnotations;

namespace Shashlik.Pager
{
    /// <summary>
    /// 分页输入
    /// </summary>
    public class PageInput
    {
        /// <summary>
        /// 分页参数,默认1
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = ("参数错误"))]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 分页参数,默认20
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = ("参数错误"))]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public SortType SortType { get; set; }
    }
}
