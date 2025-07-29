using fd.infrastructure.entity.Enums;
using fd.infrastructure.entity.SysInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.BaseProvider
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// 表名|
        /// To get the name of the table
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// DBConnection
        /// </summary>
        IDbConnection DBConnection { get; }

        IDbTransaction DBTransaction { get; set; }

        /// <summary>
        /// 开启事务|
        /// Open transaction
        /// </summary>
        IDbTransaction OpenTransaction();

        #region Sync
        /// <summary>
        /// 插入实体|
        /// Inserts an entity into table "Ts" and returns identity id or number of inserted rows if inserting a list.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>返回自增Id|Identity of inserted entity.</returns>
        long Insert(TEntity entity);

        /// <summary>
        /// 插入实体列表
        /// |Inserts an entity into table "Ts" and returns identity id or number of inserted rows if inserting a list.
        /// </summary>
        /// <param name="entityList">entity list</param>
        /// <returns>返回受影响行数|number of inserted rows if inserting a list.</returns>
        long Insert(IEnumerable<TEntity> entityList);

        /// <summary>
        /// 更新|
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        bool Update(TEntity entity);

        /// <summary>
        /// 更新部分|
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        /// <param name="entity"></param>
        /// <param name="properties">指定更新字段:x=>new {x.Name,x.Enable}</param>
        /// <param name="saveChanges">是否保存</param>
        /// <returns></returns>

        int Update(TEntity entity, Expression<Func<TEntity, object>> properties, bool saveChanges = false);

        /// <summary>
        /// 删除实体|
        /// Delete entity in table "Ts".
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>true if deleted, false if not found</returns>
        bool Delete(TEntity entity);

        /// <summary>
        /// 删除实体|
        /// Delete entity in table "Ts".
        /// </summary>
        /// <param name="id">Id of the entity to get, must be marked with [Key]/[ExplicitKey] attribute</param>
        /// <returns>Entity of T</returns>
        bool Delete(object id);

        /// <summary>
        /// 删除|
        /// Delete data in table "Ts".
        /// </summary>
        /// <param name="properties">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <returns>受影响的行数|The number of rows affected.</returns>
        bool Delete(Expression<Func<TEntity, bool>> properties, object param);

        /// <summary>
        /// 删除全部|
        /// Delete all data
        /// </summary>
        bool DeleteAll();

        /// <summary>
        /// 执行单条语句
        /// |Execute parameterized SQL.
        /// </summary>
        /// <param name="sql">parameterized SQL</param>
        /// <param name="parms">The parameters to use for this query.</param>
        /// <returns>受影响的行数|The number of rows affected.</returns>
        int Execute(string sql, dynamic parms);
     
        /// <summary>
        /// 查询单个实体|
        /// Returns a single entity by a single id from table "Ts".  
        /// Id must be marked with [Key]/[ExplicitKey] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance. 
        /// </summary>
        /// <param name="id">Id of the entity to get, must be marked with [Key]/[ExplicitKey] attribute</param>
        /// <returns>Entity of T</returns>
        TEntity Query(object id);

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        List<TEntity> Find(Expression<Func<TEntity, bool>> where, bool filterDeleted = true);

        /// <summary>
        /// 通过条件查询数据-异步
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool filterDeleted = true);

        /// <summary>
        /// 查询数据-sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="SugarParameters"></param>
        /// <returns></returns>
        List<TEntity> Find(string sql, dynamic param);

        /// <summary>
        /// 通过条件查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="selector">返回类型如:Find(x => x.UserName == loginInfo.userName, p => new { uname = p.UserName });</param>
        /// <returns></returns>
        List<T> Find<T>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, T>> selector, bool filterDeleted = true) where T : class;

        /// <summary>
        /// 通过条件查询数据-异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        ///<param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        Task<List<T>> FindAsync<T>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, T>> selector, bool filterDeleted = true) where T : class;

        /// <summary>
        /// 通过sql查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        List<TEntity> FromSql(string sql, dynamic parms);
        /// <summary>
        /// 返回单个实体
        /// </summary>
        /// <param name="predicate">查询条件</param>      
        /// <param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// </param>
        /// <returns></returns>
        TEntity FindFirst(Expression<Func<TEntity, bool>> predicate, bool filterDeleted = true);

        /// <summary>
        /// 返回单个实体-异步
        /// </summary>
        /// <typeparam name="TFind"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        Task<T> FindAsyncFirst<T>(Expression<Func<T, bool>> predicate, bool filterDeleted = true) where T : class;

        /// <summary>
        /// 返回单个实体 - 异步
        /// </summary>
        /// <param name="predicate"></param>
        ///<param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        Task<TEntity> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, bool filterDeleted = true);

        /// <summary>
        /// 返回单个实体-异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        Task<T> FindFirstAsync<T>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, T>> selector, bool filterDeleted = true);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        bool Exists(Expression<Func<TEntity, bool>> predicate, bool filterDeleted = true);

        /// <summary>
        /// 是否存在 - 异步
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="filterDeleted">是否过滤逻辑删除的数据，默认过</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, bool filterDeleted = true);

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="predicate">where条件</param>
        /// <param name="orderBy">排序字段,数据格式如:
        ///  orderBy = x => new Dictionary<object, bool>() {
        ///          { x.BalconyName,QueryOrderBy.Asc},
        ///          { x.TranCorpCode1,QueryOrderBy.Desc}
        ///         };
        /// </param>
        /// <returns></returns>
        IQueryable<TEntity> FindAsIQueryable(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, Dictionary<object, QueryOrderBy>>> orderBy = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <param name="rowcount"></param>
        /// <param name="orderBy"></param>
        /// <param name="returnRowCount"></param>
        /// <returns></returns>
        IPage<TEntity> QueryablePage(IQueryable<TEntity> queryable, int pageIndex, int pagesize, out int rowcount, Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true);

        /// <summary>
        /// 分页查询|
        /// Executes a query, returning the paging data typed as T.
        /// </summary>
        /// <param name="pageNum">页码|page number</param>
        /// <param name="pageSize">页大小|page size</param>
        /// <param name="whereString">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <param name="order">order param,(example:order:"createTime")</param>
        /// <param name="asc">Is ascending</param>
        /// <returns>返回分页数据|returning the paging data typed as T</returns>
        IPage<TEntity> QueryablePage(int pageNum, int pageSize, string whereString , object param , string order, bool asc = false);
        #endregion
    }
}
