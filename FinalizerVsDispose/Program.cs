using System;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // Main method ile çal??t?r?labilir örnek
    static void Main()
    {
        Console.WriteLine("Finalizer vs Dispose - Demo\n");

        Console.WriteLine("1) IDisposable (Dispose) - Deterministik kaynak yönetimi:");
        using (var managed = new ResourceWithDispose("using-block"))
        {
            // using blo?u sonunda Dispose() ça?r?l?r -> kaynaklar hemen b?rak?l?r
        }

        Console.WriteLine();

        Console.WriteLine("2) Finalizer (Destructor) - Non-deterministik (GC taraf?ndan temizlenir):");
        CreateWithoutDispose();

        // Zaman verip finalizer'lar?n çal??mas?n? sa?lamaya zorlayal?m (sadece demo amaçl?)
        Console.WriteLine("GC toplama ça?r?l?yor...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Thread.Sleep(200); // finalizer i? parçac???n?n mesajlar? yazmas? için küçük gecikme

        Console.WriteLine("\nDemo tamamland?. Bir tu?a bas?n...");
        Console.ReadKey();
    }

    static void CreateWithoutDispose()
    {
        var r = new ResourceWithDispose("no-dispose");
        // r.Dispose() ça?r?lmad?, nesne scope d???na ç?k?yor ve GC/Finalizer taraf?ndan temizlenecek
    }
}

/// <summary>
/// Örnek: unmanaged bellek tahsisi yapan bir s?n?f
/// Dispose pattern'i uygular; finalizer "güvenlik a??" olarak yer al?r.
/// </summary>
public class ResourceWithDispose : IDisposable
{
    private IntPtr _nativeMemory; // unmanaged resource (örnek için)
    private bool _disposed;
    private readonly string _name;

    public ResourceWithDispose(string name)
    {
        _name = name;
        // 1 KB unmanaged bellek ay?ral?m (sadece demo)
        _nativeMemory = Marshal.AllocHGlobal(1024);
        Console.WriteLine($"[{_name}] Native memory allocated: {_nativeMemory}");
    }

    // Public Dispose - tüketicinin ça??rd??? metod (deterministik)
    public void Dispose()
    {
        Dispose(disposing: true);
        // Finalizer ça?r?s?n? iptal et -> zaten temizlendi
        GC.SuppressFinalize(this);
        Console.WriteLine($"[{_name}] Dispose() ça?r?ld? ve Finalizer iptal edildi.");
    }

    // As?l temizleme mant??? burada (Dispose pattern)
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Yönetilen kaynaklar? serbest b?rak (ör. managed IDisposable nesneleri)
            // Bu örnekte yok.
        }

        // Yönetilmeyen kaynaklar? serbest b?rak
        if (_nativeMemory != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_nativeMemory);
            Console.WriteLine($"[{_name}] Native memory freed: {_nativeMemory}");
            _nativeMemory = IntPtr.Zero;
        }

        _disposed = true;
    }

    // Finalizer (destructor) - runtime taraf?ndan ça?r?l?r, non-deterministik
    ~ResourceWithDispose()
    {
        // Sadece unmanaged kaynaklar? serbest b?rakmak için Dispose(false) ça??r?l?r.
        // Finalizer içerisinde managed nesnelere güvenme.
        Dispose(disposing: false);
        Console.WriteLine($"[{_name}] Finalizer çal??t? (Dispose(false)).");
    }
}