public interface IOrderRepository
{
    bool SaveOrder(int orderId);
}

public class OrderService
{
    private readonly IOrderRepository _repository;
    public OrderService(IOrderRepository repository) => _repository = repository;

    public string Process(int id)
    {
        if (_repository.SaveOrder(id)) return "Başarılı";
        return "Hata Oluştu";
    }
}