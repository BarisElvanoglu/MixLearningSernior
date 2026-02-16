using System;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // Main method ile çalıştırılabilir örnek
    static void Main()
    {
        Console.WriteLine("Finalizer vs Dispose - Demo\n");

        Console.WriteLine("1) IDisposable (Dispose) - Deterministik kaynak yönetimi:");
        using (var managed = new ResourceWithDispose("using-block"))
        {
            // using bloğu sonunda Dispose() çağrılır -> kaynaklar hemen bırakılır
        }

        Console.WriteLine();

        Console.WriteLine("2) Finalizer (Destructor) - Non-deterministik (GC tarafından temizlenir):");
        CreateWithoutDispose();

        // Zaman verip finalizer'ların çalışmasını sağlamaya zorlayalım (sadece demo amaçlı)
        Console.WriteLine("GC toplama çağrılıyor...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Thread.Sleep(200); // finalizer iş parçacığının mesajları yazması için küçük gecikme

        Console.WriteLine("\nDemo tamamlandı. Bir tuşa basın...");
        Console.ReadKey();
    }

    static void CreateWithoutDispose()
    {
        var r = new ResourceWithDispose("no-dispose");
        // r.Dispose() çağrılmadı, nesne scope dışına çıkıyor ve GC/Finalizer tarafından temizlenecek
    }
}

/// <summary>
/// Örnek: unmanaged bellek tahsisi yapan bir sınıf
/// Dispose pattern'i uygular; finalizer "güvenlik ağı" olarak yer alır.
/// </summary>
public class ResourceWithDispose : IDisposable
{
    private IntPtr _nativeMemory; // unmanaged resource (örnek için)
    private bool _disposed;
    private readonly string _name;

    public ResourceWithDispose(string name)
    {
        _name = name;
        // 1 KB unmanaged bellek ayıralım (sadece demo)
        _nativeMemory = Marshal.AllocHGlobal(1024);
        Console.WriteLine($"[{_name}] Native memory allocated: {_nativeMemory}");
    }

    // Public Dispose - tüketicinin çağırdığı metod (deterministik)
    public void Dispose()
    {
        Dispose(disposing: true);
        // Finalizer çağrısını iptal et -> zaten temizlendi
        GC.SuppressFinalize(this);
        Console.WriteLine($"[{_name}] Dispose() çağrıldı ve Finalizer iptal edildi.");
    }

    // Asıl temizleme mantığı burada (Dispose pattern)
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Yönetilen kaynakları serbest bırak (ör. managed IDisposable nesneleri)
            // Bu örnekte yok.
        }

        // Yönetilmeyen kaynakları serbest bırak
        if (_nativeMemory != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_nativeMemory);
            Console.WriteLine($"[{_name}] Native memory freed: {_nativeMemory}");
            _nativeMemory = IntPtr.Zero;
        }

        _disposed = true;
    }

    // Finalizer (destructor) - runtime tarafından çağrılır, non-deterministik
    ~ResourceWithDispose()
    {
        // Sadece unmanaged kaynakları serbest bırakmak için Dispose(false) çağırılır.
        // Finalizer içerisinde managed nesnelere güvenme.
        Dispose(disposing: false);
        Console.WriteLine($"[{_name}] Finalizer çalıştı (Dispose(false)).");
    }
}