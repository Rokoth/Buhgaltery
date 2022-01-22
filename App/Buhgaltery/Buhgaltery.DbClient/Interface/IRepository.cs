using Buhgaltery.DbClient.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.DbClient.Interface
{
    /// <summary>
    /// DB wrapper class' interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IEntity
    {
        /// <summary>
        /// Get model list with paging
        /// </summary>
        /// <param name="filter">filter</param>     
        /// <returns>PagedResult<T></returns>
        Contract.Model.PagedResult<T> Get(Filter<T> filter);
                
        /// <summary>
        /// Get item of model by id
        /// </summary>
        /// <param name="id">id</param>       
        /// <returns></returns>
        T Get(int id);
                
        /// <summary>
        /// add model to db
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="withSave">save after add</param>
        /// <returns></returns>
        T Add(T entity, bool withSave);

        /// <summary>
        /// delete model from db
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="withSave">save after add</param>
        /// <returns></returns>
        T Delete(T entity, bool withSave);

        /// <summary>
        /// update model at db
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="withSave">save after add</param>
        /// <returns></returns>
        T Update(T entity, bool withSave);

        void SaveChanges();
    }
}
