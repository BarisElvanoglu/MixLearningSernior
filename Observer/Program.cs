using System;
using System.Collections.Generic;

// 1. Gözlemci Arayüzü (Observer Interface)
// Bildirim alacak her nesnenin sahip olması gereken metod.
public interface ISubscriber
{
    void Update(string videoTitle);
}

// 2. Yayıncı Arayüzü (Subject Interface)
// Aboneleri ekleme, çıkarma ve onlara haber verme işlemlerini tanımlar.
public interface IYouTubeChannel
{
    void Subscribe(ISubscriber subscriber);
    void Unsubscribe(ISubscriber subscriber);
    void Notify(string videoTitle);
}

// 3. Somut Yayıncı (Concrete Subject)
public class TechChannel : IYouTubeChannel
{
    private List<ISubscriber> _subscribers = new List<ISubscriber>();

    public void Subscribe(ISubscriber subscriber) => _subscribers.Add(subscriber);
    public void Unsubscribe(ISubscriber subscriber) => _subscribers.Remove(subscriber);

    // Tüm aboneleri tek tek gezip "Yeni video geldi!" diyoruz.
    public void Notify(string videoTitle)
    {
        foreach (var sub in _subscribers)
        {
            sub.Update(videoTitle);
        }
    }

    // Video yüklendiğinde tetiklenen asıl iş mantığı
    public void UploadVideo(string title)
    {
        Console.WriteLine($"Kanal: '{title}' videosunu yüklüyor...");
        Notify(title);
    }
}

// 4. Somut Gözlemciler (Concrete Observers)
public class User : ISubscriber
{
    private string _name;
    public User(string name) => _name = name;

    public void Update(string videoTitle) =>
        Console.WriteLine($"Bildirim -> Sevgili {_name}, yeni bir video var: {videoTitle}");
}

// ---------------------------------------------------------
// MAIN - İstemci Kodu (Client)
// ---------------------------------------------------------
class Program
{
    static void Main(string[] args)
    {
        // Yayıncıyı oluştur
        TechChannel myChannel = new TechChannel();

        // Aboneleri (Gözlemcileri) oluştur
        User ahmet = new User("Ahmet");
        User ayse = new User("Ayşe");

        // Abonelikleri başlat
        myChannel.Subscribe(ahmet);
        myChannel.Subscribe(ayse);

        // Bir olay gerçekleşsin
        myChannel.UploadVideo("Design Patterns 101");

        // Ayşe abonelikten çıksın
        myChannel.Unsubscribe(ayse);

        // Yeni bir video daha gelsin
        Console.WriteLine("\n--- Yeni Güncelleme ---");
        myChannel.UploadVideo("Observer Pattern Nedir?");
    }
}