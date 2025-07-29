using fd.infrastructure.entity.Enums;
using fd.infrastructure.entity.SysInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.BaseProvider
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {

       

        public long Insert(T entity)
        {
           
        }

        public long Insert(IEnumerable<T> entityList)
        {
            throw new NotImplementedException();
        }

        public bool Update(T entity)
        {
            throw new NotImplementedException();
        }

        public int Update(T entity, Expression<Func<T, object>> properties, bool saveChanges = false)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(object id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Expression<Func<T, bool>> properties, object param)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAll()
        {
            throw new NotImplementedException();
        }

        public int Execute(string sql, dynamic parms)
        {
            throw new NotImplementedException();
        }

        public T Query(object id)
        {
            throw new NotImplementedException();
        }

        public List<T> Find(Expression<Func<T, bool>> where, bool filterDeleted = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, bool filterDeleted = true)
        {
            throw new NotImplementedException();
        }

        public List<T> Find(string sql, dynamic param)
        {
            throw new NotImplementedException();
        }

        public List<T1> Find<T1>(Expression<Func<T, bool>> predicate, Expression<Func<T, T1>> selector, bool filterDeleted = true) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task<List<T1>> FindAsync<T1>(Expression<Func<T, bool>> predicate, Expression<Func<T, T1>> selector, bool filterDeleted = true) where T1 : class
        {
            throw new NotImplementedException();
        }

        public List<T> FromSql(string sql, dynamic parms)
        {
            throw new NotImplementedException();
        }

        public T FindFirst(Expression<Func<T, bool>> predicate, bool filterDeleted = true)
        {
            throw new NotImplementedException();
        }

        public Task<T1> FindAsyncFirst<T1>(Expression<Func<T1, bool>> predicate, bool filterDeleted = true) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task<T> FindFirstAsync(Expression<Func<T, bool>> predicate, bool filterDeleted = true)
        {
            throw new NotImplementedException();
        }

        public Task<T1> FindFirstAsync<T1>(Expression<Func<T, bool>> predicate, Expression<Func<T, T1>> selector, bool filterDeleted = true)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<T, bool>> predicate, bool filterDeleted = true)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool filterDeleted = true)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindAsIQueryable(Expression<Func<T, bool>> predicate, Expression<Func<T, Dictionary<object, QueryOrderBy>>> orderBy = null)
        {
            throw new NotImplementedException();
        }

        public IPage<T> QueryablePage(IQueryable<T> queryable, int pageIndex, int pagesize, out int rowcount, Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true)
        {
            throw new NotImplementedException();
        }

        public IPage<T> QueryablePage(int pageNum, int pageSize, string whereString, object param, string order, bool asc = false)
        {
            throw new NotImplementedException();
        }

        public BaseRepository()
        {
        }

        /// <summary>
        /// 仓储基类| Base Repository
        /// </summary>
        public BaseRepository(string dbAliase = "")
        {
            this.dbAliase = dbAliase;
        }

        protected virtual string dbAliase { get; set; }

        public string TableName => throw new NotImplementedException();

        public BaseRepository(IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            this.DBConnection = dbConnection;
            this.DBTransaction = dbTransaction;
        }             

    }
}
