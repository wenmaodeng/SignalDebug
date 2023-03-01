using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDebug.Models
{
    /// <summary>
    /// 信号信息
    /// </summary>
    public class SignalInfo
    {
        /// <summary>
        /// 信号主键ID
        /// </summary>
        [PrimaryKey]
        public string SignalId { get; set; }
        /// <summary>
        /// 数据主键ID
        /// </summary>
        public string DataId { get; set; }
        /// <summary>
        /// 信号名称
        /// </summary>
        public string SignalName { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType DataType { get; set; }
        /// <summary>
        /// 信号位
        /// </summary>
        public int SignalBit { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 查询排序序号
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
    }
}
