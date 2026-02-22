# 🚀 gRPC Bi-Directional Chat System 💬

![gRPC](https://img.shields.io/badge/gRPC-Server%20%26%20Client-blueviolet?style=for-the-badge&logo=grpc)
![.NET](https://img.shields.io/badge/.NET%208.0-Framework-512BD4?style=for-the-badge&logo=dotnet)
![Status](https://img.shields.io/badge/Status-Active-success?style=for-the-badge)

Bu proje, **gRPC** teknolojisinin **Bidirectional Streaming** (Çift Yönlü Akış) özelliğini kullanarak geliştirilmiş, yüksek performanslı bir konsol tabanlı chat uygulamasıdır.

---

## 🌟 Neden gRPC Chat?

Geleneksel REST mimarisinin aksine, bu projede iletişim **kesintisiz bir boru hattı** üzerinden gerçekleşir.

* **⚡ Ultra Hızlı:** JSON yerine ikili (binary) Protobuf formatı kullanılır.
* **🔄 Tam Dubleks:** Sunucu ve istemci aynı anda birbirine veri gönderir.
* **🛡️ Tip Güvenli:** Proto dosyası sayesinde veri tipleri asla şaşmaz.



---

## 🏗️ Sistem Mimarisi

Sistem iki ana bileşenden oluşur:

### 1. 🖥️ ChatHost (Sunucu)
* İstemci bağlantılarını yönetir.
* Gelen mesajları tüm bağlı kullanıcılara **Broadcast** eder.
* **Kestrel** üzerinde HTTP/2 protokolü ile çalışır.

### 2. 👤 ChatClient (İstemci)
* Sunucuya asenkron bir kanal açar.
* Aynı anda hem klavyeyi dinler hem de sunucudan gelen mesajları ekrana basar.

---

## 📜 Sözleşme: `chat.proto`

İletişimin anayasası burada yazılıdır:

```protobuf
syntax = "proto3";
option csharp_namespace = "GrpcChatApp";

service ChatService {
  // Çift yönlü akışın kalbi:
  rpc JoinChat(stream ChatMessage) returns (stream ChatMessage);
}

message ChatMessage {
  string user = 1;      // Kim gönderdi?
  string message = 2;   // Ne dedi?
}