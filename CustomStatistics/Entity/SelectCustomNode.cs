using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomStatistics.Entity
{

    /// <summary>
    /// 2020-12-25 16:58:55
    /// 这个实体类用于缓存查询时的节点数据
    /// </summary>
    class SelectCustomNode
    {
        String conditionId;
        String conditionName;
        String conditionDBName;
        String conditionType;
        String conditionListText;//下拉框时使用的字段，也可用于文本框默认值
        String conditionDBPronoun;//查询代称，举例：当使用a替代exam_master表时，配置这个字段为a，此查询条件使用a作为前缀
        String isLike;//是否模糊查询


        public string ConditionId { get => conditionId; set => conditionId = value; }
        public string IsLike { get => isLike; set => isLike = value; }
        public string ConditionName { get => conditionName; set => conditionName = value; }
        public string ConditionDBName { get => conditionDBName; set => conditionDBName = value; }
        public string ConditionType { get => conditionType; set => conditionType = value; }
        public string ConditionListText { get => conditionListText; set => conditionListText = value; }
        public string ConditionDBPronoun { get => conditionDBPronoun; set => conditionDBPronoun = value; }
    }
}
