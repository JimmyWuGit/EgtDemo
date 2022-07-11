using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EgtDemo.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        //根据ID查询
        Task<TEntity> QueryByID(object objId);
        Task<TEntity> QueryByID(object objId, bool isUseCache = false);
        Task<List<TEntity>> QueryByIDs(object[] ids);

        //新增
        Task<int> Add(TEntity entity);

        //删除
        Task<bool> DeleteByID(object id);
        Task<bool> Delete(TEntity entity);
        Task<bool> DeleteByIDs(object[] ids);

        //更新
        Task<bool> Update(TEntity entity);
        Task<bool> Update(TEntity entity, string strWhere);
        Task<bool> Update(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "");

        //查询（如果只是根据ID查就用上面的三个按ID查询方法）
        Task<List<TEntity>> Query();
        Task<List<TEntity>> Query(string strWhere);
        Task<List<TEntity>> Query(string strWhere, string strOrderByFields);
        Task<List<TEntity>> Query(string strWhere, int intTop, string strOrderByFields);
        Task<List<TEntity>> Query(string strWhere, int pageIndex, int pageSize, string strOrderByFields);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFields);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFields);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, string strOrderByFields);
        Task<List<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int pageIndex = 0, int pageSize = 20, string strOrderByFields = null);

    }
}
