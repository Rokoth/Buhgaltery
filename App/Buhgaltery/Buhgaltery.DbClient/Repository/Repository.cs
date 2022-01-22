using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Buhgaltery.DbClient.Interface;
using Buhgaltery.DbClient.Model;
using Buhgaltery.DbClient;
using Buhgaltery.Db.Repository;

namespace Buhgaltery.DbClient.Repository
{
    /// <summary>
    /// Repository - wrapper for db works
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : class, IEntity 
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public Repository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<Repository<T>>>();
        }

        /// <summary>
        /// Метод добавления модели в базу
        /// </summary>
        /// <param name="entity">модель</param>
        /// <param name="withSave">с сохраннеием</param>
        /// <returns>модель</returns>
        public T Add(T entity, bool withSave)
        {
            return Execute((context) => {
                var item = context.Set<T>().Add(entity).Entity;
                return item;
            }, "Add", withSave);
        }

        /// <summary>
        /// Метод удаления из базы
        /// </summary>
        /// <param name="entity">модель</param>
        /// <param name="withSave">с сохраннеием</param>
        /// <returns></returns>
        public T Delete(T entity, bool withSave)
        {
            return Execute((context) => {               
                var item = context.Set<T>().Remove(entity).Entity;  
                return item;
            }, "Delete", withSave);
        }

        /// <summary>
        /// Метод получения отфильтрованного списка моделей с постраничной отдачей
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns>список моделей</returns>
        public Contract.Model.PagedResult<T> Get(Filter<T> filter)
        {
            return Execute((context) =>
            {
                var pageCount = 1;
                var all = context.Set<T>().Where(filter.Selector);              
                if (!string.IsNullOrEmpty(filter.Sort))
                {
                    all = all.OrderBy(filter.Sort);
                }
                var count = all.Count();
                List<T> result;
                if (filter.Size.HasValue)
                {
                    result = all
                        .Skip(filter.Size.Value * filter.Page ?? 0)
                        .Take(filter.Size.Value)
                        .ToList();
                    pageCount = Math.Max(((count % filter.Size.Value) == 0) ? (count / filter.Size.Value) : ((count / filter.Size.Value) + 1), 1);
                }
                else
                {
                    result = all.ToList();
                }
                return new Contract.Model.PagedResult<T>(result, pageCount);
            }, "Get", false);
        }

        /// <summary>
        /// Метод получения модели по id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public T Get(int id)
        {
            return Execute((context) => {
                return  context.Set<T>()
                    .Where(s =>s.Id == id).FirstOrDefault();
            }, "Get", false);
        }

        /// <summary>
        /// Метод обновления записи в базе
        /// </summary>
        /// <param name="entity">модель</param>
        /// <param name="withSave">с сохраннеием</param>
        /// <returns></returns>
        public T Update(T entity, bool withSave)
        {
            return Execute((context) => {                
                var item = context.Set<T>().Update(entity).Entity;
                return item;
            }, "Update", withSave);
        }

        /// <summary>
        /// Обертка выполнения запросов (обработка ошибок)
        /// </summary>
        /// <typeparam name="TEx"></typeparam>
        /// <param name="action"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private TEx Execute<TEx>(Func<DbSqLiteContext, TEx> action, string method, bool withSave)
        {
            try
            {
                var context = _serviceProvider.GetRequiredService<DbSqLiteContext>();
                var result = action(context);
                if (withSave) context.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе {method} Repository: {ex.Message} {ex.StackTrace}");               
                throw new RepositoryException($"Ошибка в методе {method} Repository: {ex.Message}");
            }
        }

        public void SaveChanges()
        {
            var context = _serviceProvider.GetRequiredService<DbSqLiteContext>();
            context.SaveChanges();
        }
    }
}
