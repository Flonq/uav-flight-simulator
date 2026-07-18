# TECHNICAL DECISIONS

Bu dosya, proje boyunca alınan önemli teknik kararları ve bu kararların gerekçelerini kaydetmek için kullanılacaktır.

Her yeni karar aşağıdaki şablona uygun şekilde eklenmelidir:

```text
Karar:
Durum:
Tarih:
Gerekçe:
Alternatifler:
Sonuçlar:
```

## Durum Açıklamaları

- **Kabul edildi:** Projede uygulanacaktır.
- **Önerildi:** Henüz kesinleştirilmemiştir.
- **Değiştirildi:** Daha sonra başka bir kararla değiştirilmiştir.
- **Reddedildi:** Değerlendirilmiş fakat kullanılmamasına karar verilmiştir.

---

## TD-001 — Unity Kullanılması

**Durum:** Kabul edildi  
**Tarih:** Proje başlangıcı

### Karar

Proje Unity oyun motoru ile geliştirilecektir.

### Gerekçe

- C# ile geliştirme yapılabilmesi
- Hızlı prototipleme imkânı
- Fizik, kamera, ses ve kullanıcı arayüzü sistemlerinin hazır olması
- Windows build sürecinin kolay olması
- Geniş dokümantasyon ve topluluk desteği
- Geliştiricinin mevcut Unity deneyimi

### Alternatifler

- Unreal Engine
- Godot
- Özel oyun motoru

### Sonuçlar

Proje mimarisi Unity bileşen yapısına uygun tasarlanacaktır. Unity'ye özel bağımlılıklar mümkün olduğunca belirli katmanlarda tutulacaktır.

---

## TD-002 — Windows Masaüstünün Birincil Platform Olması

**Durum:** Kabul edildi

### Karar

İlk sürüm yalnızca Windows masaüstü platformu için hazırlanacaktır.

### Gerekçe

- Baykar başvurusunda kolayca gösterilebilir olması
- Klavye, fare ve joystick desteğinin uygun olması
- Masaüstünde daha yüksek performans sağlanması
- Mobil optimizasyon zorunluluklarının proje kapsamını büyütmemesi

### Sonuçlar

Mobil, WebGL ve VR desteği MVP kapsamına alınmayacaktır.

---

## TD-003 — Rigidbody Tabanlı Uçuş Fiziği

**Durum:** Kabul edildi

### Karar

İHA hareketi doğrudan `Transform` değiştirmek yerine Unity `Rigidbody` sistemi kullanılarak geliştirilecektir.

### Gerekçe

- Kuvvet, tork, çarpışma ve yer çekimi davranışlarının doğal şekilde modellenebilmesi
- Kalkış ve iniş sırasında pist temasının desteklenmesi
- İleride rüzgâr ve türbülans eklenebilmesi
- Fiziksel davranışların Inspector üzerinden ayarlanabilmesi

### Alternatifler

- Transform tabanlı arcade hareket
- Tamamen özel fizik çözücüsü
- Hazır uçuş simülasyonu paketi

### Sonuçlar

- Fizik işlemleri `FixedUpdate` içinde çalıştırılacaktır.
- Kuvvet ve tork uygulamaları tek bir ana uçuş fizik bileşeninde toplanacaktır.
- Görsel model ile fizik kök nesnesi gerektiğinde ayrılacaktır.

---

## TD-004 — Yarı Gerçekçi Uçuş Modeli

**Durum:** Kabul edildi

### Karar

Uçuş modeli tamamen arcade veya profesyonel eğitim seviyesinde olmayacaktır. Kontrol edilebilirliği koruyan yarı gerçekçi bir yaklaşım kullanılacaktır.

### Gerekçe

- Projenin tek geliştirici tarafından tamamlanabilir olması
- Gerçekçi görünen fakat kullanıcıyı zorlamayan bir deneyim sağlanması
- Portföy süresinin aerodinamik araştırmasına tamamen harcanmaması
- Teknik görüşmede açıklanabilir bir fizik modeli oluşturulması

### İlk sürümde modellenecek değerler

- Motor thrust
- Lift
- Drag
- Pitch, roll ve yaw torkları
- Hıza bağlı kontrol etkinliği
- Basitleştirilmiş stall davranışı
- Yer çekimi
- Yere temas

### İlk sürümde modellenmeyecek değerler

- Gelişmiş kanat profili tabloları
- Atmosfer katmanları
- Gerçek zamanlı akışkan simülasyonu
- Üreticiye özel uçuş verileri
- Sertifikalı uçuş dinamikleri

---

## TD-005 — Unity Input System

**Durum:** Kabul edildi

### Karar

Kullanıcı girdileri eski `Input Manager` yerine Unity Input System ile yönetilecektir.

### Gerekçe

- Klavye, fare, gamepad ve joystick desteğinin aynı yapı altında yönetilebilmesi
- Action Map kullanımı
- Tuş atamalarının daha kolay değiştirilebilmesi
- Girdi okuma ile uçuş fiziğinin birbirinden ayrılması

### Önerilen Action Map'ler

```text
Aircraft
├── Pitch
├── Roll
├── Yaw
├── Throttle
├── Brake
└── Reset

Camera
├── Look
├── Zoom
├── SwitchCamera
└── TrackTarget

UI
├── Navigate
├── Submit
├── Cancel
└── Pause
```

### Sonuçlar

Girdi bileşeni yalnızca kullanıcı komutlarını okuyacak; doğrudan fizik kuvveti uygulamayacaktır.

---

## TD-006 — Modüler Bileşen Mimarisi

**Durum:** Kabul edildi

### Karar

Tek bir büyük `AircraftController` sınıfı yerine sorumlulukları ayrılmış bileşenler kullanılacaktır.

### Önerilen bileşenler

```text
AircraftInputReader
AircraftPhysics
AircraftEngine
AircraftControlSurfaces
AircraftGroundController
AircraftTelemetry
AircraftAudio
AircraftDamage
```

### Gerekçe

- Kodun okunabilirliğini artırmak
- Hata ayıklamayı kolaylaştırmak
- Her sistemin bağımsız test edilebilmesi
- Yeni özellik eklenirken mevcut sistemlerin bozulma riskini azaltmak
- Teknik görüşmede mimari kararları net şekilde açıklayabilmek

### Kural

Bir sınıfın çok sayıda bağımsız görevi yerine getirdiği fark edilirse sınıf bölünecektir.

---

## TD-007 — Kodlama Standartları

**Durum:** Kabul edildi

### Kurallar

- Sınıf ve public üye adları `PascalCase` kullanılacaktır.
- Private alanlar `_camelCase` biçiminde yazılacaktır.
- Inspector alanları mümkün olduğunda `[SerializeField] private` olacaktır.
- `FindObjectOfType`, `GameObject.Find` ve benzeri pahalı aramalar sürekli kullanılmayacaktır.
- Her sınıfın tek ve anlaşılır bir sorumluluğu olacaktır.
- Magic number yerine adlandırılmış değişken kullanılacaktır.
- Fizik kodu `FixedUpdate` içinde çalışacaktır.
- Kullanıcı girdisi ile fizik uygulaması birbirinden ayrılacaktır.
- Kritik Inspector referansları `Awake` veya `OnValidate` içinde kontrol edilecektir.
- Console hataları görmezden gelinmeyecektir.
- Gereksiz `public` alan kullanılmayacaktır.

### Namespace önerisi

```csharp
namespace MertKaan.UAVSimulator
{
}
```

Alt sistemler için:

```csharp
MertKaan.UAVSimulator.Aircraft
MertKaan.UAVSimulator.CameraSystem
MertKaan.UAVSimulator.Missions
MertKaan.UAVSimulator.Telemetry
MertKaan.UAVSimulator.UI
```

---

## TD-008 — ScriptableObject Kullanımı

**Durum:** Önerildi

### Karar

Değişmeyen veya farklı araçlarda tekrar kullanılacak ayarlar için `ScriptableObject` kullanılacaktır.

### Potansiyel kullanım alanları

- İHA fizik ayarları
- Motor ayarları
- Kamera ayarları
- Görev tanımları
- Waypoint ayarları
- Ses profilleri

### Gerekçe

- Kod ile ayar verilerini ayırmak
- Farklı İHA konfigürasyonlarını kolayca oluşturmak
- Inspector üzerinden ayar yönetimini kolaylaştırmak

### Risk

Proje küçük kalırsa gereğinden fazla ScriptableObject kullanımı karmaşıklık oluşturabilir. Yalnızca tekrar kullanılacak veri gruplarında tercih edilecektir.

---

## TD-009 — Render Pipeline Seçimi

**Durum:** Önerildi

### Önerilen karar

Universal Render Pipeline kullanılması.

### Gerekçe

- Windows için yeterli görsel kalite
- Built-in Render Pipeline'a göre modern iş akışı
- HDRP'ye göre daha düşük sistem gereksinimi
- Performans ve kalite arasında dengeli yapı
- Sis, post-processing ve çevre efektleri için yeterli özellik

### Alternatifler

- Built-in Render Pipeline
- High Definition Render Pipeline

### Kesinleştirme kriteri

İHA modeli ve çevre assetleri incelendikten sonra materyal uyumluluğu kontrol edilecektir.

---

## TD-010 — Kullanıcı Arayüzü Teknolojisi

**Durum:** Önerildi

### Seçenekler

- UI Toolkit
- UGUI

### Önerilen yaklaşım

İlk prototipte hızlı geliştirme için UGUI; karmaşık yer kontrol paneli gerekiyorsa UI Toolkit değerlendirilmesi.

### Karar kriterleri

- Telemetri ekranının karmaşıklığı
- Harita ve kamera paneli ihtiyacı
- Kullanılacak hazır UI assetleri
- Geliştirme hızı
- Responsive tasarım ihtiyacı

Kesin karar verildiğinde bu bölüm güncellenecektir.

---

## TD-011 — Kamera Sisteminin Uçuş Sisteminden Ayrılması

**Durum:** Kabul edildi

### Karar

Kamera davranışları İHA fizik kodunun içine yazılmayacaktır.

### Planlanan kamera modları

- Chase Camera
- Body Camera
- Free Camera
- EO/Target Camera

### Gerekçe

- Kamera geçişlerini kolaylaştırmak
- Uçuş sistemini kamera bağımlılığından kurtarmak
- Her kameranın farklı kontrol mantığına sahip olabilmesi

---

## TD-012 — Görev Sisteminin Veri Odaklı Tasarlanması

**Durum:** Önerildi

### Karar

Görevler doğrudan sahne koduna gömülmek yerine veri üzerinden tanımlanacaktır.

### Örnek görev verileri

- Görev adı
- Açıklama
- Başlangıç konumu
- Waypoint listesi
- Hedef bölgesi
- Zaman sınırı
- Başarı koşulları
- Başarısızlık koşulları

### Gerekçe

Yeni görevlerin kod değiştirilmeden oluşturulabilmesini sağlamak.

---

## TD-013 — Git Çalışma Düzeni

**Durum:** Kabul edildi

### Karar

Tüm geliştirme adımları Git ile takip edilecektir.

### Temel kurallar

- `main` dalı çalışır durumda tutulacaktır.
- Büyük özellikler ayrı branch üzerinde geliştirilecektir.
- Her commit tek bir anlamlı değişiklik içerecektir.
- Console hatası bulunan sürüm mümkün olduğunca commit edilmeyecektir.
- Commit mesajları açıklayıcı olacaktır.

### Commit örnekleri

```text
feat: add basic aircraft throttle control
feat: implement waypoint detection
fix: prevent aircraft drifting on runway
refactor: split input logic from flight physics
docs: update project overview
```

---

## TD-014 — Harici Paket Kullanım Politikası

**Durum:** Kabul edildi

### Karar

Harici paketler yalnızca geliştirme süresini anlamlı ölçüde azalttığında kullanılacaktır.

### Kurallar

- Paketin lisansı kontrol edilecektir.
- Projenin temel mekanikleri tamamen hazır bir uçuş paketine teslim edilmeyecektir.
- Kaynağı bilinmeyen kodlar projeye eklenmeyecektir.
- Kullanılan bütün paketler README içinde belirtilecektir.
- Ücretli asset dosyaları açık GitHub deposuna yüklenmeyecektir.

### Amaç

Portföyde gösterilen ana teknik sistemlerin geliştirici tarafından yazılması.

---

## TD-015 — Performans Hedefi

**Durum:** Kabul edildi

### Karar

Proje orta seviye bir Windows bilgisayarda 1080p çözünürlükte kararlı 60 FPS hedefleyecektir.

### Temel önlemler

- Gereksiz fizik bileşenlerinden kaçınmak
- Update içinde pahalı aramalar yapmamak
- Uzak çevre nesnelerinde LOD kullanmak
- Gölge mesafesini kontrol etmek
- Gereksiz gerçek zamanlı ışıkları azaltmak
- Profiler ile darboğazları ölçmek

---

## TD-016 — Test Yaklaşımı

**Durum:** Kabul edildi

### Karar

Her sistem küçük test senaryoları ile doğrulanacaktır.

### Test türleri

- Inspector ayar testi
- Play Mode davranış testi
- Girdi testi
- Fizik kararlılık testi
- Görev başarı ve başarısızlık testi
- Build testi
- Farklı FPS değerlerinde davranış testi

### Kural

Yeni bir sistem eklenmeden önce mevcut sistemin çalışır hâli commit edilmelidir.

---

## Karar Bekleyen Konular

- [ ] Unity sürümünün tam numarası
- [ ] URP veya Built-in Render Pipeline seçimi
- [ ] UI Toolkit veya UGUI seçimi
- [ ] Kullanılacak İHA modelinin kesinleştirilmesi
- [ ] İlk sürümde joystick desteği
- [ ] Yakıt sistemi veya batarya sistemi
- [ ] Harita çözümü
- [ ] Test framework kapsamı
- [ ] Cinemachine kullanımı
- [ ] Terrain veya modüler çevre kullanımı

---

## Değişiklik Günlüğü

| Tarih | Karar | Değişiklik |
|---|---|---|
| Proje başlangıcı | TD-001 – TD-016 | İlk teknik karar taslağı oluşturuldu |
