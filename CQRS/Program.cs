using System;
using System.Collections.Generic;
using System.Linq;

namespace CqrsDemo
{
    // ==========================================================
    // 1. MODELLER VE DTO'LAR (Data Transfer Objects)
    // ==========================================================
    public record Product(int Id, string Name, decimal Price); // Veritabanı modelimiz
    public record ProductDto(string Name, string DisplayPrice); // Okuma tarafı için optimize edilmiş model

    // ==========================================================
    // 2. COMMAND TARAFI (Yazma İşlemleri)
    // "Sistemin durumunu değiştiren ancak veri dönmeyen işlemler"
    // ==========================================================

    // Command Nesnesi: Bir ürünü oluşturmak için gereken veriler
    public record CreateProductCommand(string Name, decimal Price);

    // Command Handler: Yazma mantığının işletildiği yer
    public class ProductCommandHandler
    {
        public void Handle(CreateProductCommand command)
        {
            // Gerçek senaryoda burada DbContext.Add() ve SaveChanges() olur.
            Console.WriteLine($"[COMMAND] Çalıştı: {command.Name} veritabanına eklendi.");
        }
    }

    // ==========================================================
    // 3. QUERY TARAFI (Okuma İşlemleri)
    // "Sistemi değiştirmeyen, sadece veri dönen işlemler"
    // ==========================================================

    // Query Nesnesi: Filtreleme kriterlerini tutar
    public record GetProductByIdQuery(int Id);

    // Query Handler: Okuma mantığının (Sorgulama) işletildiği yer
    public class ProductQueryHandler
    {
        public ProductDto Handle(GetProductByIdQuery query)
        {
            // Gerçek senaryoda burada veritabanından hızlıca (Dapper veya EF ile) veri çekilir.
            // Örnek olması için statik bir veri dönüyoruz:
            var dummyProduct = new Product(query.Id, "Oyuncu Bilgisayarı", 45000);

            return new ProductDto(
                Name: dummyProduct.Name,
                DisplayPrice: $"{dummyProduct.Price:C2}" // Fiyatı formatlayarak dönüyoruz
            );
        }
    }

    // ==========================================================
    // 4. ANA PROGRAM (Main)
    // ==========================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- CQRS Deseni Uygulama Örneği ---\n");

            // --- YAZMA (COMMAND) AKIŞI ---
            // Bir ürün eklemek istediğimizde sadece CommandHandler'ı kullanırız.
            var command = new CreateProductCommand("Laptop", 25000);
            var commandHandler = new ProductCommandHandler();
            commandHandler.Handle(command);

            Console.WriteLine("-----------------------------------");

            // --- OKUMA (QUERY) AKIŞI ---
            // Bir veriyi ekranda göstermek istediğimizde sadece QueryHandler'ı kullanırız.
            var query = new GetProductByIdQuery(123);
            var queryHandler = new ProductQueryHandler();
            var result = queryHandler.Handle(query);

            Console.WriteLine($"[QUERY] Sonucu: Ürün Adı: {result.Name} | Fiyatı: {result.DisplayPrice}");

            Console.WriteLine("\nİşlem tamamlandı. Çıkmak için bir tuşa basın.");
            Console.ReadKey();
        }
    }
}