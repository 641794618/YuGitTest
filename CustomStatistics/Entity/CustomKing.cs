using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomStatistics.Entity
{
    class CustomKing
    {
        //这个实体类存储所有XML配置数据
        List<CustomXmlEntity> customXmls = new List<CustomXmlEntity>();

        internal List<CustomXmlEntity> CustomXmls { get => customXmls; set => customXmls = value; }


    }
}
