using System.Linq.Expressions;
using MongoDB.Driver;
using Play.Common.Abstractions.Repositories;
using Play.Common.Entities;

namespace Play.Common.Concretes;

public sealed class MongoRepository<T> : IBaseRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> dbCollection;
    private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        dbCollection = database.GetCollection<T>(collectionName);
    }


    public async Task<IReadOnlyCollection<T>> GetAll(Expression<Func<T, bool>> filter = null)
    {
        IReadOnlyCollection<T> response;
        response = filter is null
         ? await dbCollection.Find(filterBuilder.Empty).ToListAsync()
         : await dbCollection.Find(filter).ToListAsync();

        return response;
    }

    public async Task<T> GetBy(Expression<Func<T, bool>> filter)
    {
        return await dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T> Get(Guid id)
    {
        FilterDefinition<T> filter = filterBuilder.Eq(x => x.Id, id);
        return await dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task Create(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await dbCollection.InsertOneAsync(entity);
    }

    public async Task Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        FilterDefinition<T> filter = filterBuilder.Eq(x => x.Id, entity.Id);
        await dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task Delete(Guid id)
    {
        FilterDefinition<T> filter = filterBuilder.Eq(x => x.Id, id);
        await dbCollection.DeleteOneAsync(filter);
    }
}
