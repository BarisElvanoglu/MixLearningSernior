///*
//C#’ta “kaç çeşit attribute var?” sorusunun pratik cevabı:
//Attribute’lar “çeşit” olarak isim isim (Obsolete, Serializable, etc.) sonsuzdur,
//ama TEMEL SINIFLANDIRMA “NEREYE UYGULANDIĞI (TARGET)” üzerinden yapılır.

//Aşağıda hem target çeşitlerini, hem de kullanım şekillerini tek dosyada örnekledim.
//*/

//using System;
//using System.ComponentModel.DataAnnotations;

//// =======================================================
//// 1) CUSTOM ATTRIBUTE NASIL YAZILIR? (AttributeUsage ile)
//// =======================================================

//// AttributeUsage: attribute’un NEREYE konabileceğini belirler.
//[AttributeUsage(
//    AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter,
//    AllowMultiple = true,    // Aynı hedefte birden fazla kez kullanılabilir mi?
//    Inherited = true         // Türeyen sınıflara miras kalsın mı?
//)]
//public sealed class AuditAttribute : Attribute
//{
//    // "Named arguments" için set edilebilir property’ler:
//    public string? Category { get; set; }
//    public bool Enabled { get; set; } = true;

//    // "Positional arguments" için ctor parametreleri:
//    public string Action { get; }

//    public AuditAttribute(string action) => Action = action;
//}

//// Sadece method ve return value üzerinde kullanılabilen örnek attribute
//[AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue)]
//public sealed class RangeHintAttribute : Attribute
//{
//    public int Min { get; }
//    public int Max { get; }
//    public RangeHintAttribute(int min, int max) { Min = min; Max = max; }
//}

//// =======================================================
//// 2) TARGET (NEREYE KONUR) ÇEŞİTLERİ + KULLANIM ŞEKİLLERİ
//// =======================================================

//// 2.1) Assembly / Module target (dosya seviyesinde)
//// Bu attribute’lar dosyanın en üstünde de yazılabilir.
//// [assembly: CLSCompliant(true)]
//// [module: CLSCompliant(true)]
//[assembly: CLSCompliant(true)]

//namespace Demo
//{
//    // 2.2) Class üzerine attribute
//    [Audit("class-created", Category = "Security")]
//    public class UserService
//    {
//        // 2.3) Field üzerine attribute (AttributeTargets.Field)
//        [Audit("field-track", Category = "Data")]
//        private readonly string _internalTag = "v1";

//        // 2.4) Property üzerine attribute (AttributeTargets.Property)
//        // DataAnnotations örneği (Validation amaçlı):
//        [Required(ErrorMessage = "Email zorunludur")]
//        [EmailAddress(ErrorMessage = "Email formatı hatalı")]
//        [Audit("prop-access", Category = "Validation", Enabled = true)]
//        public string Email { get; set; } = "";

//        // 2.5) Method üzerine attribute (AttributeTargets.Method)
//        // Built-in örnek: Obsolete -> derleme zamanı uyarısı/hataya çevirebilir
//        [Obsolete("Use CreateUserV2 instead", error: false)]
//        [Audit("create-user", Category = "Business")]
//        public void CreateUser(string name)
//        {
//            Console.WriteLine($"User created: {name}");
//        }

//        // 2.6) Parameter üzerine attribute (AttributeTargets.Parameter)
//        // Parametreye özel audit + validation:
//        [Audit("create-user-v2", Category = "Business")]
//        public void CreateUserV2(
//            [Required] string name,
//            [Range(1, 120)] int age
//        )
//        {
//            Console.WriteLine($"User created: {name}, age={age}");
//        }

//        // 2.7) Return value üzerine attribute (AttributeTargets.ReturnValue)
//        // return: prefix’i return değerine attribute koymak içindir.
//        [return: RangeHint(0, 10)]
//        [Audit("score-calc", Category = "Metrics")]
//        public int CalculateScore(int input)
//        {
//            // basit örnek
//            return Math.Clamp(input, 0, 10);
//        }
//    }

//    // 2.8) Interface üzerine attribute (AttributeTargets.Interface) (genelde metadata amaçlı)
//    [AttributeUsage(AttributeTargets.Interface)]
//    public sealed class ServiceContractAttribute : Attribute { }

//    [ServiceContract]
//    public interface IAccountService
//    {
//        [Audit("login")]
//        void Login(string user);
//    }

//    // 2.9) Enum / Struct / Delegate / Event target’ları da vardır:
//    [AttributeUsage(AttributeTargets.Enum)]
//    public sealed class EnumInfoAttribute : Attribute
//    {
//        public string Description { get; }
//        public EnumInfoAttribute(string description) => Description = description;
//    }

//    [EnumInfo("User roles enum")]
//    public enum Role
//    {
//        Admin,
//        User
//    }

//    // 2.10) Attribute kullanımıyla ilgili “kullanım şekilleri” özeti:
//    // - [Attr]                       => parametresiz
//    // - [Attr("positional")]         => ctor (positional) argüman
//    // - [Attr(Name="x", Flag=true)]  => named argüman (set edilebilir property/field)
//    // - [Attr][OtherAttr]            => birden çok attribute
//    // - [return: Attr]               => return value için
//    // - [assembly: Attr] / [module: Attr] => dosya seviyesi

//    public static class Program
//    {
//        public static void Main()
//        {
//            var svc = new UserService();
//            svc.CreateUser("Barış");
//            svc.CreateUserV2("Barış", 28);
//            Console.WriteLine($"Score={svc.CalculateScore(99)}");
//        }
//    }
//}

///*
//KISA ÖZET:
//- Attribute’ların “çeşidi” pratikte 2 şekilde anlaşılır:
//  (A) Target çeşitleri: Assembly, Module, Class, Struct, Enum, Interface, Method, Property, Field, Parameter, ReturnValue, Event, Delegate...
//  (B) Kullanım şekilleri: parametresiz / positional / named / çoklu / return: / assembly: / module:
//*/
