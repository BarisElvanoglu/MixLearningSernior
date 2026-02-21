using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicQueryDemo
{
    public record Product(string Name, decimal Price, string Category);

    class Program
    {
        static void Main(string[] args)
        {
            var products = new List<Product>
            {
                new("Laptop", 20000, "Elektronik"),
                new("Mouse", 500, "Elektronik"),
                new("Elma", 20, "Gıda"),
                new("Ekmek", 10, "Gıda")
            };

            // Senaryo: Kullanıcı hem "Elektronik" kategorisini seçti 
            // hem de fiyatın 1000'den büyük olmasını istedi.

            
            var p = Expression.Parameter(typeof(Product), "p");

            // 1. Filtre: p => p.Category == "Elektronik"
            var filter1 = Expression.Equal(
                Expression.Property(p, "Category"),
                Expression.Constant("Elektronik")
            );

            // 2. Filtre: p => p.Price > 1000
            var filter2 = Expression.GreaterThan(
                Expression.Property(p, "Price"),
                Expression.Constant(100m)
            );

            // 3. İKİ FİLTREYİ BİRLEŞTİR (AND)
            // p => (p.Category == "Elektronik") AND (p.Price > 1000)
            var combinedBody = Expression.And(filter1, filter2);
            var finalExpression = Expression.Lambda<Func<Product, bool>>(combinedBody, p);

            Console.WriteLine($"Oluşturulan Dinamik Sorgu: {combinedBody}");
            // 4. SONUÇ
            Console.WriteLine($"Oluşturulan Dinamik Sorgu: {finalExpression}");

            var result = products.AsQueryable().Where(finalExpression).ToList();

            Console.WriteLine("\n--- Sonuçlar ---");
            result.ForEach(x => Console.WriteLine($"{x.Name} - {x.Category} - {x.Price}TL"));
        }
    }
}