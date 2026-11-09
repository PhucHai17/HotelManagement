namespace DAL.Interfaces;

public interface IGenericRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    int Add(T entity);
    bool Update(T entity);
    bool Delete(int id);

}

    
