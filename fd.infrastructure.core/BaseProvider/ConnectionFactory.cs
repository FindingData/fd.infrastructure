using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.BaseProvider
{
    public class ConnectionFactory
    {
        private static ConcurrentDictionary<string, string> DBSettingDic;

        public const string DefaultAliase = "BasDb";


        public static void ConfigRegister(string dbAliase = DefaultAliase)
        {
            if (DBSettingDic == null)
            {
                DBSettingDic = new ConcurrentDictionary<string, string>();
            }
            if (string.IsNullOrEmpty(dbAliase))
            {
                dbAliase = DefaultAliase;
            }
            if (DBSettingDic.ContainsKey(dbAliase))
            {
                throw new Exception("The same key already exists:" + dbAliase);
            }
            DBSettingDic[dbAliase] = ConfigurationManager.ConnectionStrings[dbAliase].ConnectionString;
        }


        public static IDbConnection GetDbConnection(string dbAliase = DefaultAliase)
        {
            try
            {
                if (string.IsNullOrEmpty(dbAliase))
                {
                    dbAliase = DefaultAliase;
                }
                if (!DBSettingDic.ContainsKey(dbAliase))
                {
                    throw new Exception("The key doesn't exist:" + dbAliase);
                }
                var conn = DBSettingDic[DefaultAliase];
                return new SqlServerConect(conn);
            }
            catch (Exception ex)
            {
                throw new Exception("连接数据库过程中发生错误，检查服务器是否正常连接字符串是否正确", ex);
            }
        }
    }
}
