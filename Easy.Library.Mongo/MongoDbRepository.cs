using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace Easy.Library.Mongo
{
    public class MongoDbRepository<T> : IDisposable where T : class, new()
    {
        private readonly MongoCollection<T> _mongoCollection;
        private readonly MongoGridFS _mongoGridFile;
        public MongoDbRepository(MongoDbInstance mongoInstance)
        {
            var type = typeof(T);
            string collectionName = type.Name;
            var tbAttrObj = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
            var tbAttr = tbAttrObj as TableAttribute;
            if (tbAttr != null && !string.IsNullOrEmpty(tbAttr.Name))
                collectionName = tbAttr.Name;

            this._mongoCollection = mongoInstance._mongoDatabase.GetCollection<T>(collectionName);
            this._mongoGridFile = mongoInstance._gridFs;
        }

        public IQueryable<T> GetQueryable()
        {
            return this._mongoCollection.AsQueryable();
        }
        public T Get(Expression<Func<T, bool>> filter = null)
        {
            if (null != filter)
            {
                var findQuery = Query<T>.Where(filter);
                return this._mongoCollection.FindOneAs<T>(findQuery);
            }
            return this._mongoCollection.FindOneAs<T>();
        }

        public T Get(Expression<Func<T, bool>> filter, Dictionary<string, sbyte> sortByMap)
        {
            MongoCursor<T> cursor = null;
            if (null != filter)
            {
                var findQuery = Query<T>.Where(filter);
                cursor = this._mongoCollection.FindAs<T>(findQuery);
            }
            else
            {
                cursor = this._mongoCollection.FindAllAs<T>();
            }

            if (null != sortByMap)
            {
                var orderByQuery = new SortByDocument(sortByMap);
                cursor = cursor.SetSortOrder(orderByQuery);
            }

            return cursor.FirstOrDefault();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sortByMap"> var sort = new SortByDocument { { "_id", -1 } }; ->这里1为ASC, -1为DESC</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public List<T> PageList(Expression<Func<T, bool>> filter, Dictionary<string, sbyte> sortByMap = null, int? pageIndex = null, int? pageSize = null, string[] fields = null)
        {
            MongoCursor<T> cursor = null;

            if (null != filter)
            {
                var findQuery = Query<T>.Where(filter);
                cursor = this._mongoCollection.FindAs<T>(findQuery);
            }
            else
            {
                cursor = this._mongoCollection.FindAllAs<T>();
            }

            if (null != sortByMap)
            {
                var orderByQuery = new SortByDocument(sortByMap);
                cursor = cursor.SetSortOrder(orderByQuery);
            }
            if (fields != null && fields.Any())
            {
                cursor = cursor.SetFields(fields);
            }
            if (pageIndex.HasValue)
            {
                if (!pageSize.HasValue)
                {
                    pageSize = 30;
                }

                List<T> list = cursor.SetSkip((pageIndex.Value - 1) * pageSize.Value).SetLimit(pageSize.Value).ToList();
                return list;
            }
            return cursor.ToList();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="filterList"></param>
        /// <param name="sortByMap"> var sort = new SortByDocument { { "_id", -1 } }; ->这里1为ASC, -1为DESC</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<T> PageList(List<Expression<Func<T, bool>>> filterList, Dictionary<string, sbyte> sortByMap = null, int? pageIndex = null, int? pageSize = null, string[] fields = null)
        {
            MongoCursor<T> cursor = null;

            if (null != filterList && filterList.Count > 0)
            {
                var findQueryList = (from expression in filterList where null != expression select Query<T>.Where(expression)).ToList();

                var findQuery = Query.And(findQueryList);
                cursor = this._mongoCollection.FindAs<T>(findQuery);
            }
            else
            {
                cursor = this._mongoCollection.FindAllAs<T>();
            }

            if (null != sortByMap)
            {
                var orderByQuery = new SortByDocument(sortByMap);
                cursor = cursor.SetSortOrder(orderByQuery);
            }
            if (fields != null && fields.Any())
            {
                cursor = cursor.SetFields(fields);
            }
            if (pageIndex.HasValue)
            {
                if (!pageSize.HasValue)
                {
                    pageSize = 30;
                }

                List<T> list = cursor.SetSkip((pageIndex.Value - 1) * pageSize.Value).SetLimit(pageSize.Value).ToList();
                return list;
            }
            return cursor.ToList();
        }
        /// <summary>
        /// 获取列表top几
        /// </summary>
        /// <param name="sortByMap"> var sort = new SortByDocument { { "_id", -1 } }; ->这里1为ASC, -1为DESC</param>
        /// <returns></returns>
        public List<T> List(Expression<Func<T, bool>> filter,Dictionary<string, sbyte> sortByMap = null, int? top = null)
        {
            MongoCursor<T> cursor = null;

            if (null != filter)
            {
                var findQuery = Query<T>.Where(filter);
                cursor = this._mongoCollection.Find(findQuery);
            }
            else
            {
                cursor = this._mongoCollection.FindAll();
            }

            if (null != sortByMap)
            {
                var orderByQuery = new SortByDocument(sortByMap);
                cursor = cursor.SetSortOrder(orderByQuery);
            }

            if (top.HasValue)
            {
                List<T> list = cursor.SetLimit(top.Value).ToList();
                return list;
            }
            return cursor.ToList();
        }
        /// <summary>
        /// 查询数据记录数
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public int Count(params Expression<Func<T, bool>>[] filters)
        {
            if (null != filters && filters.Length > 0)
            {
                var findQueryList = (from expression in filters where null != expression select Query<T>.Where(expression)).ToList();

                var findQuery = Query.And(findQueryList);
                long count = this._mongoCollection.Count(findQuery);
                return (int)count;
            }
            else
            {
                long count = this._mongoCollection.Count();
                return (int)count;
            }
        }

        /// <summary>
        /// 插入操作
        /// </summary>
        /// <param name="entity"></param>
        public bool Insert(T entity)
        {
            WriteConcernResult result = this._mongoCollection.Insert(entity);
            return result.Ok;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public IEnumerable<WriteConcernResult> InsertBatch(IEnumerable<T> entitys)
        {
            return this._mongoCollection.InsertBatch(entitys);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="updateMap"></param>
        /// <returns></returns>
        public T FindAndUpdate(Expression<Func<T, bool>> filter, IMongoUpdate updateMap)
        {
            var finder = Query<T>.Where(filter);
            var result = this._mongoCollection.FindAndModify(finder, null, updateMap);
            var entity = result.GetModifiedDocumentAs<T>();
            return entity;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="updateMap"></param>
        /// <param name="uFlag"></param>
        public bool Update(Expression<Func<T, bool>> filter, Dictionary<string, object> updateMap, string uFlag = "$set")
        {
            return Update(filter, updateMap, UpdateFlags.Multi, uFlag);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="filter">更新条件</param>
        /// <param name="updateMap"></param>
        /// <param name="updateFlags"></param>
        /// <param name="uFlag"></param>
        public bool Update(Expression<Func<T, bool>> filter, Dictionary<string, object> updateMap, UpdateFlags updateFlags, string uFlag = "$set")
        {
            var findQuery = Query<T>.Where(filter);
            var updateQuery = new UpdateDocument { { uFlag, new QueryDocument(updateMap) } };
            WriteConcernResult result = this._mongoCollection.Update(findQuery, updateQuery, updateFlags);
            return result.Ok;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="updateMap"></param>
        /// <param name="updateFlags"></param>
        public bool Update(Expression<Func<T, bool>> filter, UpdateDocument updateMap, UpdateFlags updateFlags = UpdateFlags.Multi)
        {
            var findQuery = Query<T>.Where(filter);
            WriteConcernResult result = this._mongoCollection.Update(findQuery, updateMap, updateFlags);
            return result.Ok;
        }

        public bool Update(T entity)
        {
            WriteConcernResult result = this._mongoCollection.Save(entity);
            return result.Ok;
        }

        public bool Delete(Expression<Func<T, bool>> filter)
        {
            var findQuery = Query<T>.Where(filter);
            WriteConcernResult result = this._mongoCollection.Remove(findQuery);
            return result.Ok;
        }

        public int Sum(Expression<Func<T, bool>> filter, string filedName)
        {
            var findQuery = Query<T>.Where(filter);
            string mapFunc = @"function(){emit(0, this." + filedName + ");};";

            string reduceFunc = @"function(g, videoScore){  
                                    var total = 0;
                                    total = Array.sum(videoScore);
                                    return total;
                                };";

            var results = this._mongoCollection.MapReduce(findQuery, mapFunc, reduceFunc).GetResults();
            if (results.Any())
            {
                var item = results.FirstOrDefault();
                var sum = item.GetValue("value").ToInt32();
                return sum;
            }
            return 0;
        }
        public int Sum(List<Expression<Func<T, bool>>> filters, string filedName)
        {
            var findQueryList = new List<IMongoQuery>();
            foreach (var expression in filters)
            {
                if (null != expression)
                    findQueryList.Add(Query<T>.Where(expression));
            }

            var findQuery = Query.And(findQueryList);
            string mapFunc = @"function(){emit(0, this." + filedName + ");};";

            string reduceFunc = @"function(g, videoScore){  
                                    var total = 0;
                                    total = Array.sum(videoScore);
                                    return total;
                                };";

            var results = this._mongoCollection.MapReduce(findQuery, mapFunc, reduceFunc).GetResults();
            if (results.Any())
            {
                var item = results.FirstOrDefault();
                var sum = item.GetValue("value").ToInt32();
                return sum;
            }
            return 0;
        }

        public Dictionary<string, string> MapReduce(Expression<Func<T, bool>> filter, string mapFunc, string reduceFunc)
        {
            var findQuery = Query<T>.Where(filter);
            var results = this._mongoCollection.MapReduce(findQuery, mapFunc, reduceFunc).GetResults();

            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach (var result in results)
            {
                map.Add(result.GetValue("_id").ToString(), result.GetValue("value").ToString());
            }

            return map;
        }

        public Dictionary<string, string> MapReduce(List<Expression<Func<T, bool>>> filters, string mapFunc, string reduceFunc)
        {
            var findQueryList = (from expression in filters where null != expression select Query<T>.Where(expression)).ToList();

            var findQuery = Query.And(findQueryList);
            var results = this._mongoCollection.MapReduce(findQuery, mapFunc, reduceFunc).GetResults();

            return results.ToDictionary(result => result.GetValue("_id").ToString(), result => result.GetValue("value").ToString());
        }
        /// <summary>
        /// 上传一个本地文件到mongodb数据库中去
        /// </summary>
        /// <param name="localFileName">本地文件路径</param>
        /// <returns>上传之后的文件名称</returns>
        public string SaveFile(string localFileName)
        {
            Thread.Sleep(1);
            string remoteFileName = Guid.NewGuid().ToString();
            this._mongoGridFile.Upload(localFileName, remoteFileName);
            return remoteFileName;
        }
        /// <summary>
        /// 从mongodb数据库中下载一个文件到本地文件
        /// </summary>
        /// <param name="localFileName">本地文件</param>
        /// <param name="remoteFileName">mongodb中存储的文件名</param>
        public void DownFile(string localFileName, string remoteFileName)
        {
            this._mongoGridFile.Download(localFileName, remoteFileName);
        }

        /// <summary>
        /// 从mongodb数据库中删除一个文件
        /// </summary>
        /// <param name="remoteFileName">mongodb中存储的文件名,一般是一个Guid值</param>
        public void DeleteFile(string remoteFileName)
        {
            this._mongoGridFile.Delete(remoteFileName);
        }

        public void Dispose()
        {
        }
    }
}
