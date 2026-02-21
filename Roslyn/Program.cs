using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics.Metrics;

/*
 * ROSLYN NELER YAPABİLİR? (Özet Dosyası)
 * 
 * Roslyn; C# ve Visual Basic kodlarını sadece makine diline çevirmekle kalmayıp, 
 * bu kodları bir veri yapısı olarak analiz etmenize, değiştirmenize ve derleme anında 
 * dinamik olarak yeni kodlar üretmenize olanak tanıyan açık kaynaklı ve "akıllı" bir derleyici platformudur.
 */
namespace RoslynCapabilities
{
    class RoslynDemo
    {
        static void Main()
        {
            // 1. KODU ANALİZ EDER (Parsing)
            // Bir string'i alır ve onu 'Syntax Tree' denilen anlamlı bir ağaca dönüştürür.
            string sourceCode = "class MyClass { void MyMethod() { int x = 10; } }";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);

            // 

            // 2. KODUN İÇİNDE GEZER (Navigation)
            // Kodun içindeki tüm sınıfları, metotları veya değişkenleri tek tek bulabilir.
            var root = tree.GetCompilationUnitRoot();
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            // 3. HATALARI BULUR (Diagnostics)
            // Kodda yazım hatası veya mantık hatası (noktalı virgül eksikliği vb.) var mı söyler.
            var diagnostics = tree.GetDiagnostics();

            // 4. KODU DEĞİŞTİRİR (Transformation)
            // Mevcut bir sınıfın ismini veya içeriğini programatik olarak güncelleyebilir.
            var oldClass = classes.First();
            var newClass = oldClass.WithIdentifier(SyntaxFactory.Identifier("NewClassName"));

            // 5. YENİ KOD ÜRETİR (Code Generation)
            // Hiç yoktan sıfır bir C# dosyası veya metodu inşa edebilir.
            var myMethod = SyntaxFactory.MethodDeclaration(
                                SyntaxFactory.ParseTypeName("void"), "GeneratedMethod")
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // 

            // 6. SEMANTİK ANALİZ YAPAR (Symbol/Meaning)
            // Değişkenin sadece ismini değil, hangi kütüphaneden geldiğini ve tipini anlar.
            // (Bunun için 'Compilation' nesnesi gerekir.)

            // 7. KODU ÇALIŞTIRIR (Scripting)
            // C# kodlarını derlemeden, çalışma zamanında (Runtime) "script" gibi koşturabilir.

            // 8. KURALLARI DAYATIR (Analyzers)
            // "Public metotlar büyük harfle başlamalı" gibi özel şirket kurallarını denetler.

            // 9. OTOMATİK DÜZELTME YAPAR (Code Fixes)
            // Bulduğu hataları Visual Studio'daki 'Ampul' simgesiyle otomatik düzeltebilir.

            // 10. KAYNAK ÜRETİCİLERİ (Source Generators)
            // Proje derlenirken arka planda senin yerine ek dosyalar (.g.cs) yazar.

            Console.WriteLine("Roslyn yetenekleri analiz edildi.");
        }
    }
}


//Analiz: Kodun röntgenini çeker.

//Denetim: Kod standartlarına uyulup uyulmadığını kontrol eder.

//Üretim: Senin yerine "hamallık" olan kodları (Boilerplate) yazar.

//Dönüşüm: Eski teknolojideki kodları otomatik olarak yenisine çevirir.

//Entegrasyon: IDE(Visual Studio) ile konuşarak sana akıllı ipuçları verir.