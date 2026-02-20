// Program.cs
// NuGet: Castle.Core

using System;
using System.Linq;
using Castle.DynamicProxy;

// ===============================
// 1) Attribute (Aspect) Tanımı
// ===============================
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class LogAttribute : Attribute
{
    public string? Message { get; }
    public LogAttribute(string? message = null) => Message = message;
}

// ===============================
// 2) Interceptor (AOP motoru)
//    - Metot çağrısını araya girip sarar
//    - [Log] varsa önce/sonra log atar
// ===============================
public sealed class LoggingInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        // Metodun üzerinde [Log] var mı?
        var logAttr = invocation.Method
            .GetCustomAttributes(typeof(LogAttribute), true)
            .FirstOrDefault() as LogAttribute;

        // Attribute yoksa AOP devreye girmesin, direkt çalıştır
        if (logAttr is null)
        {
            invocation.Proceed();
            return;
        }

        var methodName = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
        var argsText = string.Join(", ", invocation.Arguments.Select(a => a?.ToString() ?? "null"));

        Console.WriteLine($"[AOP-LOG] -> GİRİŞ  : {methodName}({argsText})");
        if (!string.IsNullOrWhiteSpace(logAttr.Message))
            Console.WriteLine($"[AOP-LOG] -> Not    : {logAttr.Message}");

        try
        {
            // Asıl metodu çalıştır
            invocation.Proceed();

            // void ise ReturnValue null gelir
            Console.WriteLine($"[AOP-LOG] -> ÇIKIŞ  : {methodName} | Return: {invocation.ReturnValue ?? "void"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AOP-LOG] -> HATA   : {methodName} | {ex.Message}");
            throw; // hatayı yutma, aynen fırlat
        }
    }
}

// ===============================
// 3) Servis + Interface
//    - AOP genelde interface proxy ile çalışır
// ===============================
public interface IOrderService
{
    void CreateOrder(string productId, int quantity);

    [Log("Sipariş toplamı hesaplanıyor")]
    decimal CalculateTotal(decimal unitPrice, int quantity);
}

public class OrderService : IOrderService
{
    // Attribute yok => log basmaz
    public void CreateOrder(string productId, int quantity)
    {
        Console.WriteLine($"Sipariş oluşturuldu: {productId} x{quantity}");
    }

    // Attribute var => log basar
    [Log("KDV ve indirim hesapları bu metodun içinde")]
    public decimal CalculateTotal(decimal unitPrice, int quantity)
    {
        return unitPrice * quantity * 1.20m; // örnek KDV
    }
}

// ===============================
// 4) Uygulama (Proxy üretimi)
// ===============================
public static class Program
{
    public static void Main()
    {
        var generator = new ProxyGenerator();
        var interceptor = new LoggingInterceptor();

        // OrderService yerine proxy veriyoruz
        IOrderService orderService = generator.CreateInterfaceProxyWithTarget<IOrderService>(
            new OrderService(),
            interceptor
        );

        // [Log] yok -> AOP log atmaz
        
        orderService.CreateOrder("PRD-001", 2);

        Console.WriteLine("-----");

        // [Log] var -> AOP log atar
        var total = orderService.CalculateTotal(100m, 2);
        Console.WriteLine($"Toplam: {total}");
    }
}
