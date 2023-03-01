using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDebug.Models
{
    /// <summary>
    /// 信号位数据类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 浮点类型数据
        /// </summary>
        FloatSignal,
        /// <summary>
        /// 整型类型数据
        /// </summary>
        IntSignal,
        /// <summary>
        /// 布尔类型数据
        /// </summary>
        BoolSignal,
    }
}
