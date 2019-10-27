using System.Collections.Generic;

namespace Orleans.Promotion.Grains
{
    /// <summary>
    /// 用于保存会话统计信息
    /// </summary>
    public class SessionStatistics
    {
        public int Count => LoginUsers.Count;

        public List<string> LoginUsers { get; set; } = new List<string>();
    }


}