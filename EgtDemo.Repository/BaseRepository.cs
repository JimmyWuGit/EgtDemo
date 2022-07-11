using EgtDemo.Common;
using EgtDemo.IRepository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EgtDemo.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = BaseDBConfig.ConnectionString,
            DbType = DbType.SqlServer,
            IsAutoCloseConnection = true
        });
        //internal SqlSugarClient Db { get; private set; }
        internal SimpleClient<TEntity> EntityDB { get; private set; }

        public async Task<int> Add(TEntity entity)
        {
            var result = await Task.Run(() => Db.Insertable(entity).ExecuteReturnBigIdentity());

            return (int)result;
        }

        public async Task<bool> Delete(TEntity entity)
        {
            var result = await Task.Run(() => Db.Deleteable(entity).ExecuteCommand());
            return result > 0;
        }

        public async Task<bool> DeleteByID(object id)
        {
            var result = await Task.Run(() => Db.Deleteable<TEntity>(id).ExecuteCommand());
            return result > 0;
        }

        public async Task<bool> DeleteByIDs(object[] ids)
        {
            var result = await Task.Run(() => Db.Deleteable<TEntity>().In(ids).ExecuteCommand());
            return result > 0;
        }

        public async Task<List<TEntity>> Query()
        {
            return await Task.Run(() => EntityDB.GetList());
        }

        public async Task<List<TEntity>> Query(string strWhere)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList());
        }

        public async Task<List<TEntity>> Query(string strWhere, string strOrderByFields)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFields), strOrderByFields)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList());
        }

        public async Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFields)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFields), strOrderByFields)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToList());
        }

        public async Task<List<TEntity>> Query(string strWhere, int pageIndex, int pageSize, string strOrderByFields)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFields), strOrderByFields)
                .WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToPageList(pageIndex, pageSize));
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Task.Run(() => EntityDB.GetList(whereExpression));
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFields)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFields), strOrderByFields)
                .WhereIF(whereExpression != null, whereExpression).ToList());
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFields)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFields), strOrderByFields)
                .WhereIF(whereExpression != null, whereExpression).Take(intTop).ToList());
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc)
                .WhereIF(whereExpression != null, whereExpression).ToList());
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, string strOrderByFields)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFields), strOrderByFields)
                .WhereIF(whereExpression != null, whereExpression)
                .ToPageList(pageIndex, pageSize));
        }

        public async Task<TEntity> QueryByID(object objId)
        {
            //return await Task.Run(() => EntityDB.GetById(objId));
            return await Task.Run(() => Db.Queryable<TEntity>().InSingle(objId));
        }

        public async Task<TEntity> QueryByID(object objId, bool isUseCache = false)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().WithCacheIF(isUseCache).InSingle(objId));
        }

        public async Task<List<TEntity>> QueryByIDs(object[] ids)
        {
            return await Task.Run(() => Db.Queryable<TEntity>().In(ids).ToList());
        }

        public async Task<List<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int pageIndex = 0, int pageSize = 20, string strOrderByFields = null)
        {
            return await Task.Run(() => Db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFields), strOrderByFields)
                .WhereIF(whereExpression != null, whereExpression)
                .ToPageList(pageIndex, pageSize));
        }

        public async Task<bool> Update(TEntity entity)
        {
            var result = await Task.Run(() => Db.Updateable(entity).ExecuteCommand());
            return result > 0;
        }

        public async Task<bool> Update(TEntity entity, string strWhere)
        {
            return await Task.Run(() => Db.Updateable(entity).Where(strWhere).ExecuteCommand() > 0);
        }

        public async Task<bool> Update(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "")
        {
            var updateable = await Task.Run(() => Db.Updateable(entity));
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                updateable = await Task.Run(() => updateable.IgnoreColumns(lstIgnoreColumns.ToArray()));
            }
            if(lstColumns!=null && lstColumns.Count > 0)
            {
                updateable = await Task.Run(() => updateable.UpdateColumns(lstColumns.ToArray()));
            }
            if (string.IsNullOrWhiteSpace(strWhere))
            {
                updateable = await Task.Run(() => updateable.Where(strWhere));
            }
            return await Task.Run(() => updateable.ExecuteCommand()) > 0;
        }
    }
}
