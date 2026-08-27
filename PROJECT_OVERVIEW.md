# PROJECT OVERVIEW

## 1. Proje Adı

**Baykar İş Başvurusu – İHA Uçuş Simülatörü**

> Çalışma adı: **UAV Flight Simulator**

---

## 2. Projenin Amacı

Bu proje, Unity kullanılarak geliştirilen masaüstü tabanlı bir insansız hava aracı uçuş simülatörüdür.

Projenin temel amacı; yazılım mimarisi, oyun motoru kullanımı, fizik tabanlı sistem geliştirme, kullanıcı arayüzü tasarımı, görev akışı oluşturma ve teknik dokümantasyon becerilerimi tek bir portföy çalışmasında göstermektir.

Proje, Baykar iş başvurusunda teknik portföy çalışması olarak sunulmak üzere geliştirilmektedir.

Bu çalışma resmî bir Baykar ürünü değildir ve Baykar tarafından desteklendiği veya onaylandığı iddiasını taşımaz.

---

## 3. Projenin Kısa Tanımı

Kullanıcı, sabit kanatlı bir İHA'yı yer kontrol istasyonu arayüzü üzerinden kontrol edecektir.

Simülasyonun planlanan temel akışı:

1. Görev brifinginin görüntülenmesi
2. İHA sistemlerinin hazırlanması
3. Pist üzerinde hızlanma ve kalkış
4. Belirlenen rota veya waypoint noktalarının takip edilmesi
5. Görev bölgesine ulaşılması
6. Elektro-optik kamera ile hedef bölgenin gözlemlenmesi
7. Görev hedefinin tamamlanması
8. Üs bölgesine dönüş
9. Yaklaşma ve iniş
10. Görev sonuçlarının raporlanması

---

## 4. Projenin Hedefleri

### Teknik hedefler

- Unity üzerinde modüler bir uçuş kontrol sistemi geliştirmek
- Fizik tabanlı kalkış, uçuş ve iniş davranışları oluşturmak
- Kamera ve sensör sistemlerini birbirinden bağımsız bileşenler hâlinde tasarlamak
- Görev ve waypoint sistemleri geliştirmek
- Telemetri verilerini gerçek zamanlı olarak kullanıcı arayüzünde göstermek
- Kodun okunabilir, genişletilebilir ve test edilebilir olmasını sağlamak
- Git ve GitHub üzerinden düzenli sürüm takibi yapmak
- Profesyonel seviyede proje dokümantasyonu hazırlamak

### Portföy hedefleri

- Unity ve C# yetkinliğini göstermek
- Karmaşık bir sistemi aşamalara bölerek geliştirebildiğini göstermek
- Fizik, kullanıcı arayüzü ve oyun mantığını aynı projede birleştirmek
- Teknik kararları gerekçeleriyle belgelemek
- Sonuç odaklı ve tamamlanabilir bir ürün ortaya koymak

---

## 5. Hedef Platform

- **Birincil platform:** Windows masaüstü
- **Kontrol yöntemi:** Klavye, fare ve gamepad
- **İleri aşama seçeneği:** Joystick veya HOTAS desteği
- **Ekran modu:** 16:9 çözünürlükler
- **Hedef performans:** Orta seviye bir bilgisayarda kararlı 60 FPS

---

## 6. Kullanılan Teknolojiler

| Alan | Teknoloji |
|---|---|
| Oyun motoru | Unity 6.3 LTS — 6000.3.20f1 |
| Programlama dili | C# |
| Render Pipeline | Universal Render Pipeline |
| Sürüm kontrolü | Git |
| Kod deposu | GitHub |
| Girdi sistemi | Unity Input System |
| Fizik sistemi | Unity Rigidbody tabanlı fizik |
| Kullanıcı arayüzü | uGUI ve TextMeshPro |
| Hedef platform | Windows |

### Mevcut teknik kararlar

- Proje `Universal 3D` şablonu ile oluşturulmuştur.
- Renk uzayı `Linear` olarak kullanılmaktadır.
- Kullanıcı girdileri Unity Input System üzerinden yönetilecektir.
- İHA hareketi `Rigidbody` tabanlı yarı gerçekçi bir fizik modeli ile geliştirilecektir.
- Görsel model ile fizik kök nesnesi birbirinden ayrılmıştır.
- Kullanıcı arayüzü için uGUI ve TextMeshPro kullanılacaktır.
- Cinemachine ilk prototip için gerekli görülmemiştir; kamera sistemi geliştirilirken tekrar değerlendirilecektir.
- Joystick desteği MVP sonrasında veya ihtiyaç oluşması durumunda değerlendirilecektir.

Ayrıntılı teknik kararlar `TECHNICAL_DECISIONS.md` dosyasında tutulmaktadır.

---

## 7. Planlanan Ana Sistemler

### 7.1 İHA uçuş sistemi

- Motor gücü ve throttle kontrolü
- Pitch kontrolü
- Roll kontrolü
- Yaw kontrolü
- Lift ve drag hesaplamaları
- Stall davranışı
- Maksimum ve minimum hız sınırları
- Kalkış ve iniş davranışları
- Yer hareketi
- İsteğe bağlı rüzgâr etkisi

### 7.2 Kamera sistemi

- Takip kamerası
- Serbest gözlem kamerası
- Gövde kamerası
- Elektro-optik hedefleme kamerası
- Kamera modları arasında geçiş
- Zoom ve hedef takibi

### 7.3 Görev sistemi

- Görev brifingi
- Görev başlangıç ve bitiş koşulları
- Waypoint sistemi
- Kontrol noktaları
- Hedef bölgesi
- Görev başarı ve başarısızlık koşulları
- Görev sonucu ekranı

### 7.4 Telemetri sistemi

- Hız
- İrtifa
- Dikey hız
- Yön
- Pitch, roll ve yaw değerleri
- Motor gücü
- Görev süresi
- Waypoint mesafesi

### 7.5 Yer kontrol istasyonu

- Ana uçuş ekranı
- Telemetri göstergeleri
- Mini harita veya görev haritası
- Kamera görüntüsü
- Uyarılar ve durum mesajları
- Görev kontrol paneli

### 7.6 Ses sistemi

- Motor sesi
- Rüzgâr sesi
- Tekerlek ve pist temas sesi
- Uyarı sesleri
- Kullanıcı arayüzü sesleri

---

## 8. İlk Sürüm Kapsamı — MVP

İlk tamamlanabilir sürüm aşağıdaki özellikleri içerecektir:

- Tek bir sabit kanatlı İHA
- Tek bir havaalanı veya test sahası
- Klavye, fare ve gamepad ile kontrol
- Temel fizik tabanlı uçuş
- Kalkış ve iniş
- Takip kamerası
- Basit elektro-optik kamera
- Hız ve irtifa telemetrisi
- En az üç waypoint içeren görev
- Görev başarı ekranı
- Windows çalıştırılabilir build

MVP tamamlanmadan gelişmiş özelliklere geçilmeyecektir.

---

## 9. Kapsam Dışında Tutulacak Konular

İlk sürümde aşağıdaki özellikler hedeflenmemektedir:

- Askerî seviyede gerçek uçuş modeli
- Gerçek İHA aviyoniklerinin birebir simülasyonu
- Silah sistemi
- Gerçek operasyon verileri
- Çok oyunculu yapı
- Gelişmiş yapay zekâ pilotu
- Büyük ve açık dünya haritası
- VR desteği
- Profesyonel pilot eğitim sertifikasyonu

Bu proje bir portföy ve yazılım demonstrasyon çalışmasıdır; sertifikalı uçuş eğitim simülatörü değildir.

---

## 10. Proje Klasör Yapısı

```text
Assets/
├── _Project/
│   ├── Art/
│   ├── Audio/
│   ├── Prefabs/
│   │   └── Aircraft/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Aircraft/
│   │   ├── Camera/
│   │   ├── Core/
│   │   ├── Input/
│   │   ├── Mission/
│   │   ├── Telemetry/
│   │   └── UI/
│   ├── Settings/
│   └── Tests/
└── ThirdParty/
```

Proje tarafından geliştirilen içerikler mümkün olduğunca `Assets/_Project` altında tutulmaktadır.

Harici modeller ve asset paketleri `Assets/ThirdParty` altında izole edilmektedir.

---

## 11. Geliştirme Yaklaşımı

Proje küçük ve test edilebilir aşamalar hâlinde geliştirilmektedir.

Her özellik için uygulanan temel süreç:

1. Gereksinimi tanımla
2. Mevcut sistemi incele
3. Teknik çözümü belirle
4. Küçük bir prototip oluştur
5. Unity Editor içinde test et
6. Hataları düzelt
7. Kod temizliği yap
8. Git commit oluştur
9. Dokümantasyonu güncelle

### Mimari yaklaşım

Tek bir büyük kontrol sınıfı yerine sorumlulukları ayrılmış bileşenler kullanılacaktır.

Planlanan temel bileşenler:

```text
AircraftInputReader
AircraftPhysics
AircraftEngine
AircraftControlSurfaces
AircraftGroundController
AircraftTelemetry
CameraModeController
MissionManager
Waypoint
GroundControlUI
```

Kullanıcı girdisi, fizik sistemi, kamera, telemetri, görev sistemi ve kullanıcı arayüzü birbirinden mümkün olduğunca bağımsız tutulacaktır.

---

## 12. Başarı Kriterleri

Proje aşağıdaki koşullar sağlandığında başarılı kabul edilecektir:

- Kullanıcı İHA'yı pistten kaldırabilmeli
- İHA havada kararlı şekilde kontrol edilebilmeli
- Kullanıcı waypoint rotasını takip edebilmeli
- Hedef bölgesi kamera ile gözlemlenebilmeli
- Görev tamamlandıktan sonra üsse dönülebilmeli
- İHA piste indirilebilmeli
- Temel telemetri verileri doğru görüntülenmeli
- Proje kritik hata vermeden Windows üzerinde çalışmalı
- GitHub README dosyası projeyi yeterli şekilde tanıtmalı
- Kod yapısı teknik görüşmede açıklanabilir olmalı

---

## 13. Mevcut Durum

**Mevcut aşama:** Faz 4 tamamlandı — Faz 5 Motor ve Throttle Sistemi geliştirmesine geçiliyor.

### Tamamlanan temel çalışmalar

- Unity 6.3 LTS projesi oluşturuldu ve temel proje ayarları tamamlandı.
- Universal Render Pipeline yapılandırıldı.
- Unity Input System ve TextMeshPro proje altyapısına dahil edildi.
- Modüler `_Project` klasör yapısı oluşturuldu.
- `FlightTest` ana test sahnesi hazırlandı.
- Military Base Pack kullanılarak havaalanı ve üs test ortamı oluşturuldu.
- Harici environment materyalleri URP ile uyumlu hâle getirildi.
- Sahnedeki gereksiz gerçek zamanlı gölge maliyetleri azaltıldı.
- İHA spawn noktası ve yer kontrol istasyonu için mantıksal alan belirlendi.
- İHA görsel modeli projeye aktarıldı.
- İHA görsel yönü ve ölçeği Unity sahnesine uygun hâle getirildi.
- Ayrı bir fizik kök nesnesi oluşturuldu.
- Rigidbody ve temel gövde, kanat ve iniş takımı collider yapısı oluşturuldu.
- Rigidbody için otomatik Center of Mass kullanımı doğrulandı.
- Yerçekimi ve pist temas davranışı Play Mode'da test edildi.
- Test sahnesinde Console hatasız ve uyarısız çalışacak duruma getirildi.
- Faz 2 ve Faz 3 geliştirmeleri `main` branch'ine birleştirildi.
- `Aircraft`, `Camera` ve `UI` Action Map'lerini içeren Input System yapısı oluşturuldu.
- `AircraftInputReader` ile kullanıcı girdisi fizik sisteminden ayrıldı.
- Klavye ve DualSense gamepad kontrolleri Play Mode'da doğrulandı.
- EO zoom için fare ve gamepad girdileri hazırlandı.
- Girdi değerlerini doğrulamak için geliştirme amaçlı debug paneli oluşturuldu.
- Özel joystick/HOTAS desteğinin MVP sonrasına ertelenmesine karar verildi.

### Sıradaki geliştirme

Bir sonraki aşama **Faz 5 — Motor ve Throttle Sistemi** olacaktır.

Bu aşamada:

- `AircraftEngine` bileşeni oluşturulacak
- Throttle girdisi motor sistemine bağlanacak
- Motor thrust değeri hesaplanacak
- Rigidbody üzerine ileri yönlü kuvvet uygulanacak
- Motor ve throttle değerleri Inspector üzerinden ayarlanabilir tutulacak
- Sistem uçuş aerodinamiğinden bağımsız olarak test edilecektir

---

## 14. Proje Sahibi

**Geliştirici:** Mert Kaan  
**Rol:** Yazılım Mühendisi / Unity Geliştiricisi  
**Proje türü:** Bireysel portföy projesi
