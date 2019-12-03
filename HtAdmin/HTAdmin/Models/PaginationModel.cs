
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class PaginationModel
    {
        /// <summary>
        /// 分页索引号，从1开始
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 分页数据一页数据量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总的数据量
        /// </summary>
        public int TotalCount { get; set; }

        public int PageCount
        {
            get
            {
                if (PageSize <= 0)
                {
                    return 0;
                }
                if ((TotalCount % PageSize) > 0)
                {
                    return TotalCount / PageSize + 1;
                }
                else
                {
                    return TotalCount / PageSize;
                }
            }
        }

    }
}