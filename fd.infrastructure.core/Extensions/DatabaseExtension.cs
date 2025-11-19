using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{
    public static class DatabaseExtension
    {
        /// <summary>
        /// 通用视图查询：传入视图名称、WHERE 子句和匿名对象参数。
        /// WHERE 子句中用 :ParamName 作为占位符（Oracle 风格）。
        /// </summary>
        public static Task<List<dynamic>> QueryViewDynamicAsync(
         this DbContext db,
         string viewName,
         string whereClause = null,
         IDictionary<string, object> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(viewName))
                throw new ArgumentNullException(nameof(viewName));

            var sql = new StringBuilder();
            sql.Append("SELECT * FROM ").Append(viewName);

            if (!string.IsNullOrWhiteSpace(whereClause))
            {
                sql.Append(" WHERE ").Append(whereClause);
            }

            return DynamicListFromSqlAsync(db,sql.ToString(), parameters);
        }

        public static async Task<List<dynamic>> DynamicListFromSqlAsync(
       this DbContext db,
       string sql,
       IDictionary<string, object> parameters = null)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));

            var result = new List<dynamic>();

            var conn = db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (var kv in parameters)
                    {
                        var p = cmd.CreateParameter();
                        // 注意：Oracle 参数名不要带冒号，SQL 里写 :P_NAME，这里写 "P_NAME"
                        p.ParameterName = kv.Key;
                        p.Value = kv.Value ?? DBNull.Value;
                        cmd.Parameters.Add(p);
                    }
                }

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new ExpandoObject() as IDictionary<string, object>;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i); // 视图列名/别名，可中文
                            var value = await reader.IsDBNullAsync(i)
                                ? null
                                : reader.GetValue(i);

                            row[name] = value;
                        }

                        result.Add(row);
                    }
                }
            }

            return result;
        }

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
