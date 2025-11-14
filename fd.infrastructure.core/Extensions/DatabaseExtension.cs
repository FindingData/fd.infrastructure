using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; 

namespace fd.infrastructure.core.Extensions
{
    public static class DatabaseExtension
    {

      

     

        public static IEnumerable<dynamic> DynamicListFromSql(this DbContext db, string Sql, Dictionary<string, object> Params)
        {            
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = Sql;
                if (cmd.Connection.State != ConnectionState.Open) { cmd.Connection.Open(); }

                foreach (KeyValuePair<string, object> p in Params)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = p.Key;
                    dbParameter.Value = p.Value;
                    cmd.Parameters.Add(dbParameter);
                }

                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var row = new ExpandoObject() as IDictionary<string, object>;
                        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                        {
                            row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                        }
                        yield return row;
                    }
                }
            }
        }


        /// 执行存储过程：无结果集（支持 OUT/INOUT）
        public static async Task<int> ExecProcAsync(
            this DbContext db,
            string procName,
            IEnumerable<DbParameter> parameters = null,
            CancellationToken ct = default)
        {
            await using var conn = db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = procName;
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                foreach (var p in parameters) cmd.Parameters.Add(p);

            return await cmd.ExecuteNonQueryAsync(ct);
        }


        /// 执行存储过程：无结果集（支持 OUT/INOUT）
        public static int ExecProc(
            this DbContext db,
            string procName,
            Dictionary<string, object> parameters = null)
        {
            using var conn = db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = procName;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (KeyValuePair<string, object> p in parameters)
            {
                DbParameter dbParameter = cmd.CreateParameter();
                dbParameter.ParameterName = p.Key;
                dbParameter.Value = p.Value;
                cmd.Parameters.Add(dbParameter);
            }
            return cmd.ExecuteNonQuery();
        }

        public static IEnumerable<dynamic> DynamicListFromSp(this DbContext db, string storedProcedureName, Dictionary<string, object> Params)
        {
          
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                if (cmd.Connection.State != ConnectionState.Open) { cmd.Connection.Open(); }

                foreach (KeyValuePair<string, object> p in Params)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = p.Key;
                    dbParameter.Value = p.Value;
                    cmd.Parameters.Add(dbParameter);
                }
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var row = new ExpandoObject() as IDictionary<string, object>;
                        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                        {
                            var fieldName = dataReader.GetName(fieldCount);
                            var fieldValue = dataReader[fieldCount];
                            // 检查字段类型，并确保精确的类型转换
                            if (fieldValue is decimal)
                            {
                                row.Add(fieldName, (decimal)fieldValue);
                            }
                            else if (fieldValue is double)
                            {
                                row.Add(fieldName, Convert.ToDecimal(fieldValue)); // 如果数据库中是double，将其转换为decimal
                            }
                            else
                            {
                                row.Add(fieldName, fieldValue);
                            }
                            //row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                        }
                        yield return row;
                    }
                }
            }
        }

    }
}
