namespace DAL.Interfaces;

using System.Collections.Generic;

/// <summary>
/// Interface Generic Repository định nghĩa các thao tác CRUD chuẩn.
/// </summary>
/// <typeparam name="T">Kiểu thực thể</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary> Lấy toàn bộ danh sách. </summary>
    List<T> GetAll();

    /// <summary> Tìm thực thể theo Id. </summary>
    T? GetById(int id);

    /// <summary> Thêm mới thực thể. </summary>
    bool Add(T entity);

    /// <summary> Cập nhật thực thể. </summary>
    bool Update(T entity);

    /// <summary> Xóa thực thể theo Id. </summary>
    bool Delete(int id);
}
