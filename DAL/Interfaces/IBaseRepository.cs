namespace DAL.Interfaces;

// Interface cơ sở định nghĩa CRUD chuẩn
public interface IBaseRepository<T> where T : class
{
    List<T> GetAll();
    T? GetById(int id);
    bool Add(T entity);
    bool Update(T entity);
    bool Delete(int id);
}
