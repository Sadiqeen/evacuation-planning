namespace EvacuationPlanning.Repositories.Interfaces
{
    public interface IBaseRepository<TEntity>
    {
        public Task<List<TEntity>> GetAllAsync();
        public Task<TEntity?> GetByIdAsync(string id);
        public Task<TEntity> InsertAsync(TEntity user);
        public Task<TEntity> UpdateAsync(TEntity user);
    }
}