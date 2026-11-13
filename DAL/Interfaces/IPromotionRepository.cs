namespace DAL.Interfaces;

// Interface repository khuyến mãi
public interface IPromotionRepository : IGenericRepository<Promotion>
{
    Promotion? GetByCode(string code);
}
