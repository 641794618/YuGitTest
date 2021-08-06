using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomStatistics.Entity
{
    /// <summary>
    /// 2020-12-25 16:32:13 
    /// 映射类---用于程序加载时将xml中的内容填充到实体类中
    /// 否则每次都取一遍---太麻烦了
    /// 2020-12-25 16:33:38
    /// 完全重构暂时搁置---暂时用一个单次查询的类
    /// 2021-07-22 16:35:58
    /// 必须一个承载数据的类才能存储数据,进行相关业务的继续
    /// 
    /// </summary>
    class CustomXmlEntity
    {
        String customId;
        String customTitle;
        String customAuthor;
        String customDate;
        String customSql_statement;
        String isNeedConditionKey;
        List<SelectCustomNode> selectConditions;

        public string CustomId { get => customId; set => customId = value; }
        public string CustomTitle { get => customTitle; set => customTitle = value; }
        public string CustomAuthor { get => customAuthor; set => customAuthor = value; }
        public string CustomDate { get => customDate; set => customDate = value; }
        public string CustomSql_statement { get => customSql_statement; set => customSql_statement = value; }
        public string IsNeedConditionKey { get => isNeedConditionKey; set => isNeedConditionKey = value; }
        internal List<SelectCustomNode> SelectConditions { get => selectConditions; set => selectConditions = value; }
    }
}
