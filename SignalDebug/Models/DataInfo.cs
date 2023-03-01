using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDebug.Models
{
    /// <summary>
    /// 数据信息
    /// </summary>
    public class DataInfo
    {
        /// <summary>
        /// 数据主键ID
        /// </summary>
        [PrimaryKey]
        public string DataId { get; set; }
        /// <summary>
        /// 数据名称
        /// </summary>
        public string DataName { get; set; }
        /// <summary>
        /// 帧头
        /// </summary>
        public string FrameHead { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Lenth { get; set; }
    }
}
