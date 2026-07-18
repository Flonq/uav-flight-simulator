# PROJECT OVERVIEW

## 1. Proje Adı

**Baykar İş Başvurusu – İHA Uçuş Simülatörü**

> Çalışma adı: **UAV Flight Simulator**

---

## 2. Projenin Amacı

Bu proje, Unity kullanılarak geliştirilecek masaüstü tabanlı bir insansız hava aracı uçuş simülatörüdür.

Projenin temel amacı; yazılım mimarisi, oyun motoru kullanımı, fizik tabanlı sistem geliştirme, kullanıcı arayüzü tasarımı, görev akışı oluşturma ve teknik dokümantasyon becerilerimi tek bir portföy çalışmasında göstermektir.

Proje, Baykar iş başvurusunda teknik portföy çalışması olarak sunulmak üzere geliştirilecektir.

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
- **Kontrol yöntemi:** Klavye ve fare
- **İleri aşama seçeneği:** Gamepad veya joystick desteği
- **Ekran modu:** 16:9 çözünürlükler
- **Hedef performans:** Orta seviye bir bilgisayarda kararlı 60 FPS

---

## 6. Kullanılacak Teknolojiler

| Alan | Teknoloji |
|---|---|
| Oyun motoru | Unity |
| Programlama dili | C# |
| Sürüm kontrolü | Git |
| Kod deposu | GitHub |
| Geliştirme ortamı | Visual Studio veya JetBrains Rider |
| Girdi sistemi | Unity Input System |
| Fizik sistemi | Unity Rigidbody tabanlı fizik |
| Kullanıcı arayüzü | Unity UI Toolkit veya UGUI |
| Hedef platform | Windows |

### Henüz kesinleştirilmemiş bilgiler

- Kullanılacak Unity sürümünün tam numarası
- Render Pipeline seçimi
- Kullanılacak İHA modelinin kesin adı
- UI Toolkit veya UGUI tercihi
- Joystick desteğinin ilk sürüme dahil edilip edilmeyeceği

Bu kararlar verildiğinde `TECHNICAL_DECISIONS.md` dosyasına eklenecektir.

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
- Kokpit veya gövde kamerası
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
- Yakıt veya enerji seviyesi
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
- Klavye ve fare ile kontrol
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

## 10. Önerilen Proje Klasör Yapısı

```text
Assets/
├── _Project/
│   ├── Art/
│   │   ├── Aircraft/
│   │   ├── Environment/
│   │   ├── Materials/
│   │   └── UI/
│   ├── Audio/
│   ├── Prefabs/
│   │   ├── Aircraft/
│   │   ├── Environment/
│   │   ├── Mission/
│   │   └── UI/
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
├── Plugins/
└── ThirdParty/
```

---

## 11. Geliştirme Yaklaşımı

Proje küçük ve test edilebilir aşamalar hâlinde geliştirilecektir.

Her özellik için uygulanacak temel süreç:

1. Gereksinimi tanımla
2. Mevcut sistemi incele
3. Teknik çözümü belirle
4. Küçük bir prototip oluştur
5. Unity Editor içinde test et
6. Hataları düzelt
7. Kod temizliği yap
8. Git commit oluştur
9. Dokümantasyonu güncelle

Kod ve Unity ayarları geliştirici tarafından manuel olarak uygulanacaktır.

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

**Durum:** Planlama ve dokümantasyon aşaması

Şu anda:

- Projenin genel amacı belirlendi
- Ana sistemler taslak olarak tanımlandı
- İlk sürüm kapsamı oluşturuldu
- Dokümantasyon yapısı hazırlandı

Bir sonraki adım:

- Unity sürümünü kesinleştirmek
- Yeni Unity projesini oluşturmak
- Git deposunu hazırlamak
- Temel klasör yapısını oluşturmak
- İlk test sahnesini hazırlamak

---

## 14. İletişim ve Proje Sahibi

**Geliştirici:** Mert Kaan  
**Rol:** Yazılım Mühendisi / Unity Geliştiricisi  
**Proje türü:** Bireysel portföy projesi
