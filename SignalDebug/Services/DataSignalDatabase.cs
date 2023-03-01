using SignalDebug.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDebug.Services
{
    public class DataSignalDatabase
    {
        SQLiteAsyncConnection Database;
        public DataSignalDatabase()
        {
        }

        #region 数据库初始化
        /// <summary>
        /// 数据库初始化
        /// </summary>
        /// <returns></returns>
        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await Database.CreateTableAsync<DataInfo>();
            await Database.CreateTableAsync<SignalInfo>();
        }
        #endregion 

        #region 数据操作

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataInfo>> GetDataInfosAsync()
        {
            await Init();
            return await Database.Table<DataInfo>().ToListAsync();
        }

        /// <summary>
        /// 添加或更新数据信息
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <returns></returns>
        public async Task<int> SaveDataInfoAsync(DataInfo dataInfo)
        {
            await Init();
            if (!string.IsNullOrEmpty(dataInfo.DataId))
            {
                return await Database.UpdateAsync(dataInfo);
            }

            else
            {
                dataInfo.DataId = Guid.NewGuid().ToString();
                return await Database.InsertAsync(dataInfo);
            }

        }
        /// <summary>
        /// 查询数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DataInfo> GetDataInfoAsync(string id)
        {
            await Init();
            return await Database.Table<DataInfo>().Where(i => i.DataId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 删除数据信息
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <returns></returns>
        public async Task<int> DeleteDataInfoAsync(DataInfo dataInfo)
        {
            await Init();
            return await Database.DeleteAsync(dataInfo);
        }

        #endregion

        #region 信号操作

        /// <summary>
        /// 查询信号列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<SignalInfo>> GetSignalInfosAsync(string id)
        {
            await Init();
            List<SignalInfo> signals = new List<SignalInfo>();
            var tempSignals = await Database.Table<SignalInfo>()?.ToListAsync() ?? null;
            if (tempSignals != null)
                signals = tempSignals.Where(s => s.DataId == id).ToList();
            return signals;
        }

        /// <summary>
        /// 添加或更新信号
        /// </summary>
        /// <param name="signalInfo"></param>
        /// <returns></returns>
        public async Task<int> SaveSignalInfoAsync(SignalInfo signalInfo)
        {
            await Init();
            if (!string.IsNullOrEmpty(signalInfo.SignalId))
                return await Database.UpdateAsync(signalInfo);
            else
            {
                signalInfo.SignalId = Guid.NewGuid().ToString();
                return await Database.InsertAsync(signalInfo);
            }

        }
        /// <summary>
        /// 查询信号信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SignalInfo> GetSignalInfoAsync(string id)
        {
            await Init();
            return await Database.Table<SignalInfo>().Where(i => i.DataId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 删除信号信息
        /// </summary>
        /// <param name="signalInfo"></param>
        /// <returns></returns>
        public async Task<int> DeleteSignalInfoAsync(SignalInfo signalInfo)
        {
            await Init();
            return await Database.DeleteAsync(signalInfo);
        }

        #endregion
    }
}
