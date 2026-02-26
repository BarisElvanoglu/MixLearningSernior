#nullable enable // Nullable Reference Types aktif

using System;

public record Circle(double Radius);
public record Rectangle(double Width, double Height);

public class Program
{
    public static void Main()
    {
        // Nullable referans: Değer null gelebilir.
        object? shape = GetRandomShape();

        Console.WriteLine("--- 1. Yöntem: IF Bloğu (Type Pattern) ---");
        HandleWithIf(shape);

        Console.WriteLine("\n--- 2. Yöntem: SWITCH Expression (Property Pattern) ---");
        HandleWithSwitch(shape);
    }

    // 1. YÖNTEM: IF-ELSE (Geleneksel ama güvenli)
    public static void HandleWithIf(object? shape)
    {
        // Nullable kontrolü ve tip eşleme aynı anda yapılır.
        if (shape is Circle c)
        {
            Console.WriteLine($"Çemberin Alanı: {Math.PI * c.Radius * c.Radius:F2}");
        }
        else if (shape is Rectangle r)
        {
            Console.WriteLine($"Dikdörtgenin Alanı: {r.Width * r.Height}");
        }
        else // 'shape' null ise veya başka bir tip ise buraya düşer (Default)
        {
            Console.WriteLine("Geçersiz şekil veya Null değer!");
        }
    }

    // 2. YÖNTEM: SWITCH Expression (Modern ve Deklaratif)
    public static void HandleWithSwitch(object? shape)
    {
        string result = shape switch
        {
            Circle c => $"Çember Alanı: {Math.PI * c.Radius * c.Radius:F2}",
            Rectangle r => $"Dikdörtgen Alanı: {r.Width * r.Height}",
            null => "Hata: Şekil null geldi!", // Nullable disiplini bunu zorunlu kılar
            _ => "Bilinmeyen bir şekil tipi." // Default case
        };

        Console.WriteLine(result);
    }

    // Rastgele şekil veya null döndüren yardımcı metod
    public static object? GetRandomShape()
    {
        int r = new Random().Next(0, 3);
        return r switch { 0 => new Circle(5), 1 => new Rectangle(10, 5), _ => null };
    }
}