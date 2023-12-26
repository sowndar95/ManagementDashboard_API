using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.Json;
using ManagementDashboard_Entities;
using ManagementDashboard_Entities.Base;

namespace ManagementDashboard_Services
{
    public abstract class BaseService<T> where T : ManagementDashboardEntityBase, new()
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly IMongoDatabase _database;
        protected readonly ApplicationSettings _applicationSettings;

        public BaseService(ApplicationSettings settings)
        {
            _applicationSettings = settings;

            var client = new MongoClient(_applicationSettings.DatabaseSettings.ConnectionString);
            _database = client.GetDatabase(_applicationSettings.DatabaseSettings.Database);

            T temp = new T();
            _collection = _database.GetCollection<T>(temp.CollectionName);
        }

        #region Get All
        public virtual async Task<IList<T>> GetAll()
        {
            return await _collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }
        #endregion

        #region Find
        public async Task<IList<T>> Find(Expression<Func<T, bool>> expression)
        {
            FilterDefinition<T> filterDefinition = Builders<T>.Filter.Where(expression);
            return await _collection.FindAsync<T>(filterDefinition).Result.ToListAsync();
        }
        public async Task<T> FindOne(Expression<Func<T, bool>> expression)
        {
            FilterDefinition<T> filterDefinition = Builders<T>.Filter.Where(expression);
            return await _collection.FindAsync<T>(filterDefinition).Result.FirstOrDefaultAsync();
        }

        public virtual async Task<T> Find(Guid id)
        {
            FilterDefinition<T> filterDefinition = Builders<T>.Filter.Eq(_a => _a.Id, id);
            return await _collection.FindAsync<T>(filterDefinition).Result.FirstOrDefaultAsync();
        }

        #endregion

        #region Insert & Insert Many
        public virtual async Task<T> Insert(T data)
        {
            data.ModifiedDate = DateTime.Now;

            if (data.Id == Guid.Empty)
            {
                await _collection.InsertOneAsync(data);
            }
            else
            {
                FilterDefinition<T> filterDefinition = Builders<T>.Filter.Eq(l => l.Id, data.Id);
                await _collection.ReplaceOneAsync(filter: filterDefinition,
                                                    options: new ReplaceOptions { IsUpsert = true },
                                                    replacement: data
                                                );
            }
            return data;
        }

        public virtual async Task<IEnumerable<T>> InsertMany(IEnumerable<T> data) //Create
        {
            var dataToAdd = data.Where(d => d.Id == Guid.Empty);
            var dataToUpdate = data.Where(d => d.Id != Guid.Empty).ToList();

            //Bulk Insert the New Records, if any records is present
            if (dataToAdd.Any())
                await _collection.InsertManyAsync(dataToAdd);

            //Bulk Update the Existing Records
            var bulkUpdateInformation = new List<WriteModel<T>>();

            foreach (var item in dataToUpdate)
            {
                var filterDefinition = Builders<T>.Filter.Eq(p => p.Id, item.Id);
                bulkUpdateInformation.Add(new ReplaceOneModel<T>(filterDefinition, item));
            }

            if (dataToUpdate.Any())
            {
                await _collection.BulkWriteAsync(bulkUpdateInformation, new BulkWriteOptions() { IsOrdered = false });
            }
            return data;
        }
        #endregion

        #region Delete
        public async Task<bool> Delete(Guid id)
        {
            FilterDefinition<T> filterDefinition = Builders<T>.Filter.Eq(l => l.Id, id);
            var result = await _collection.DeleteOneAsync(filterDefinition);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteMany(Expression<Func<T, bool>> expression)
        {
            FilterDefinition<T> filterDefinition = Builders<T>.Filter.Where(expression);
            var result = await _collection.DeleteManyAsync(filterDefinition);
            return result.DeletedCount > 0;
        }
        #endregion
    }
}




