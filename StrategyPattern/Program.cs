using System;

// 1. Strateji Arayüzü (Strategy Interface)
// Tüm ödeme yöntemlerinin (algoritmaların) uyması gereken ortak imza.
public interface IPaymentStrategy
{
    void ProcessPayment(double amount);
}

// 2. Somut Stratejiler (Concrete Strategies)
// "Ödeme yapma" işini farklı şekillerde gerçekleştiren sınıflar.
public class CreditCardPayment : IPaymentStrategy
{
    public void ProcessPayment(double amount) =>
        Console.WriteLine($"{amount} TL Kredi Kartı ile ödendi. Banka onayı alındı.");
}

public class BitcoinPayment : IPaymentStrategy
{
    public void ProcessPayment(double amount) =>
        Console.WriteLine($"{amount} TL Bitcoin ile ödendi. Blockchain onayı bekleniyor...");
}

// 3. Bağlam Sınıfı (Context)
// İstemcinin kullandığı ana sınıf. Hangi stratejinin seçildiğini bilir ama detayına karışmaz.
public class ShoppingCart
{
    private IPaymentStrategy _paymentStrategy;

    // Çalışma anında (runtime) stratejiyi değiştirmemizi sağlar (SetStrategy).
    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy;
    }

    public void Checkout(double total)
    {
        if (_paymentStrategy == null)
            Console.WriteLine("Lütfen önce bir ödeme yöntemi seçin!");
        else
            _paymentStrategy.ProcessPayment(total);
    }
}

// ---------------------------------------------------------
// MAIN - İstemci Kodu (Client)
// ---------------------------------------------------------
class Program
{
    static void Main(string[] args)
    {
        ShoppingCart cart = new ShoppingCart();

        // Senaryo 1: Kullanıcı Kredi Kartı seçti
        cart.SetPaymentStrategy(new CreditCardPayment());
        cart.Checkout(1500.50);

        // Senaryo 2: Kullanıcı vazgeçti, Bitcoin ile ödemek istiyor (Dinamik değişim!)
        cart.SetPaymentStrategy(new BitcoinPayment());
        cart.Checkout(1500.50);
    }
}