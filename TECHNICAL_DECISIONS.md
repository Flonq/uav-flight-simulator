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
AircraftControlSurfaceAnimator
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

**Durum:** Kabul edildi  
**Tarih:** 2026-09-06 öncesinde doğrulandı

### Karar

Universal Render Pipeline (URP) kullanılacaktır.

### Gerekçe

- Windows için yeterli görsel kalite
- Built-in Render Pipeline'a göre modern iş akışı
- HDRP'ye göre daha düşük sistem gereksinimi
- Performans ve kalite arasında dengeli yapı
- Sis, post-processing ve çevre efektleri için yeterli özellik

### Alternatifler

- Built-in Render Pipeline
- High Definition Render Pipeline

### Sonuçlar

- Proje Unity 6000.3.20f1 üzerinde URP ile kurulmuştur.
- Military Base Pack materyalleri `URP/Lit` ile uyumlu hâle getirilmiştir.
- HDRP ve Built-in Render Pipeline MVP kapsamı dışındadır.

---

## TD-010 — Kullanıcı Arayüzü Teknolojisi

**Durum:** Kabul edildi  
**Tarih:** 2026-09-06 öncesinde doğrulandı

### Seçenekler

- UI Toolkit
- UGUI

### Karar

İlk sürümde uGUI ve TextMeshPro kullanılacaktır.

### Karar kriterleri

- Telemetri ekranının karmaşıklığı
- Harita ve kamera paneli ihtiyacı
- Kullanılacak hazır UI assetleri
- Geliştirme hızı
- Responsive tasarım ihtiyacı

Mevcut pasif input debug arayüzü bu teknolojiyle oluşturulmuş ve doğrulanmıştır.

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

## TD-017 — Unity Sürümü ve Temel Proje Ayarları

**Durum:** Kabul edildi  
**Tarih:** 2026-09-06 öncesinde doğrulandı

### Karar

```text
Unity:               6000.3.20f1 / Unity 6.3 LTS
Platform:            Windows 64-bit
Color Space:         Linear
Asset Serialization: Force Text
```

### Sonuçlar

- Yeni paket veya API önerileri bu Unity sürümüyle uyumlu olmalıdır.
- Proje sürümü, paketler ve ayarlar canlı repository üzerinden denetlenmeden değiştirilmemelidir.

---

## TD-018 — Özel Blender İHA Modeli ve Görsel/Fizik Ayrımı

**Durum:** Kabul edildi  
**Tarih:** 2026-09-11; Unity doğrulaması 2026-09-12

### Karar

Blender 5.2.1 LTS ile sıfırdan geliştirilen özel İHA modeli ana görsel yön olarak kullanılacaktır. Model; ayrı aileronlar, ayrı ruddervatorlar, pusher pervane ve iniş takımı içerir ve geliştirici tarafından modelleme açısından tamamlanmış olarak onaylanmıştır.

Unity yapısı korunacaktır:

```text
AircraftRoot
└── VisualPivot
```

### Sonuçlar

- Görsel model fizik kökünü sürmez; simülasyon durumunu görsel olarak takip eder.
- Model `Assets/_Project/Art/Aircraft/CustomUAV/UAV_Custom.fbx` yoluna temiz export olarak alınmıştır; referans/helper/cutter/guide nesneleri runtime hiyerarşisine taşınmamıştır.
- Metre ölçeği, Unity yerel `-Z` burun yönü, pivotlar, normaller, URP materyali ve hareketli parça hiyerarşisi doğrulanmıştır.
- `PF_CustomUAVVisual` ile `PF_CustomUAVAircraftPrototype` ayrı tutulmuştur.
- Doğrulanmış özel model aktif hâle getirilmiş, önceki çalışan Meshy görseli geri dönüş için inactive `AircraftRoot_Meshy_Backup` olarak korunmuştur.
- Import doğrulamasında 33 renderer/benzersiz mesh, 44.922 triangle ve `(12.14, 1.84, 6.79)` görsel boyutu ölçülmüştür.

---

## TD-019 — Üçüncü Taraf Military Base Pack Politikası

**Durum:** Kabul edildi  
**Tarih:** 2026-09-06

### Karar

Military Base Pack yerel geliştirme ortamında tutulacak, fakat lisans nedeniyle açık Git deposuna eklenmeyecektir.

### Sonuçlar

```text
Assets/ThirdParty/Tiny Teacup Studio/Military Base Pack/
```

klasörü ve `.meta` dosyası `.gitignore` kapsamında kalmalıdır. Eski Git referansları veya ham asset dosyaları yeniden repository geçmişine sokulmamalıdır.

---

## TD-020 — Motor ve Aerodinamik Kuvvet Sorumlulukları

**Durum:** Kabul edildi  
**Tarih:** 2026-09-11

### Karar

- `AircraftInputReader` yalnızca ham kullanıcı komutlarını sağlar.
- `AircraftEngine`, throttle durumu, throttle rampası, motor açık/kapalı durumu, RPM, thrust hesabı ve propulsion kuvvetinden sorumludur.
- `AircraftPhysics`, lift, drag ve aerodinamik dönme kuvvetlerinden sorumludur.
- `AircraftGroundController`, fren ve yer hareketi davranışından sorumludur.
- Rigidbody kuvvetleri fixed-timestep yolunda uygulanır.

### Gerekçe

Throttle durumunun input ve motor bileşenlerinde iki kez sahiplenilmesini önlemek ve her sistemin tek sorumlulukla test edilebilmesini sağlamak.

---

## TD-021 — Özel UAV Fizik Prefabı ve Görsel Kontrol Yüzeyi Sınırı

**Durum:** Kabul edildi

**Tarih:** 2026-09-12

### Karar

- Özel UAV fizik prototipi tek kök `Rigidbody` ve çocuklarda 10 primitive collider kullanan compound collider yapısında tutulacaktır.
- Elle ayarlanmış colliderlar otomatik prefab yeniden oluşturma sırasında korunacaktır.
- `AircraftControlSurfaceAnimator` yalnızca input komutlarını görsel yüzey sapmalarına dönüştürür; fizik kuvveti, throttle state, RPM veya propulsion sahiplenmez.
- Aileronlar ve ruddervatorlar yerel X ekseninde; `Rotor_Pivot` yerel Y ekseninde döner.
- Rotorun sürekli dönüş hızı ileride `AircraftEngine` tarafından üretilen RPM verisinden beslenecektir.

### Doğrulama

- Her iki aileronda yerel `+15°`, arka kenarı yukarı taşır.
- Her iki ruddervatorda yerel `+15°`, yüzeyi yukarı taşır.
- `Rotor_Pivot` yerel Y pozitif yönde saat yönünün tersine ve merkezden kaymadan döner.
- `FlightTest` Play Mode testinde uçak sıçramadan, savrulmadan ve zeminden geçmeden üç teker üzerinde kararlı kalmıştır.

### Açık kalibrasyon

`Rigidbody.mass = 100`, Automatic Center of Mass ve Automatic Tensor mevcut prototip değerleridir. Gerçek araç kütlesi ve ağırlık merkezi verisi olmadan fiziksel doğruluk iddiası taşımaz.

---

## TD-022 — Runtime Bileşenlerinin Prefab Sahipliği

**Durum:** Kabul edildi ve uygulandı
**Tarih:** 2026-09-15

### Karar

`AircraftInputReader` ve `AircraftControlSurfaceAnimator`, `PF_CustomUAVAircraftPrototype` kökünde tutulur. Animator, aynı prefabın Input Reader ve dört kontrol yüzeyine referans verir. `InputDebugPanel` sahnede kalır ve aktif uçak instance'ına bağlanır.

### Gerekçe ve alternatif

Sahneye eklenen bileşenler yerine prefab sahipliği seçildi; yeni instance, input ve görsel kontrol yüzeyleri için eksiksiz bağlantılarla oluşturulur.

### Sonuçlar

FlightTest added-component override'ları prefab üzerine uygulandı. Entegrasyon aracı prefabın runtime bağlantılarını doğrular, yeni prototip oluşturma yolunda bu bileşenleri bağlar ve staging sırasında eski input bileşenini kopyalamaz. Mevcut collider ve Rigidbody ayarları korunur. Throttle state/ramp aktarımı ayrı görevdir.

### Doğrulama (2026-09-15)

- Unity Editor içinde derleme ve Play Mode başlangıcı: hata/uyarı yok.
- FlightTest'te added-component override sayısı 0; debug panel aktif prefab instance'ına bağlı.
- Geçici yeni runtime instance'ında tek Input Reader/Animator ve instance içi referanslar doğrulandı.
- Kontrollü sanal gamepad testi: pitch=-1, roll=1, yaw=1; aileronlar -20/+20°, ruddervatorlar -5/-25°. Bu test Input System güncellemesini ve bileşen metotlarını kontrollü çağırır; fiziksel klavye/gamepad ile kullanıcı testi değildir.
- Yeni prototip runtime bağlantı kurucusu geçici preview sahnesinde, mevcut prefab doğrulayıcısı asset üzerinde başarıyla çalıştı.
- Rigidbody ve 10 collider'ın serialized blokları HEAD ile birebir aynı.
- Geçici model inceleme sahnesindeki ek smoke test bu görevde çalıştırılmadı; sahne daha sonra TD-029 ile kaldırıldı.

---

## TD-023 — Throttle Komutu ve Motor Durumunun Ayrılması

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-15

### Karar

`AircraftInputReader.ThrottleInput` yalnızca -1…+1 kullanıcı komutunu sağlar. `AircraftEngine.Throttle`, 0…1 aralığındaki tek throttle durumudur. Engine, `FixedUpdate` içinde komut × değişim hızı × fizik adımı kadar artış/azalış uygular. Varsayılan başlangıç 0 ve değişim hızı 0,5/s olarak korunmuştur.

### Sonuçlar

- Komut sıfır olduğunda throttle korunur; sınırlar 0 ve 1'dir.
- Input Reader devre dışı kalınca gaz komutu sıfırlanır; Engine mevcut throttle değerini korur.
- Engine yeniden etkinleştiğinde throttle korunur; yeni instance başlangıç değerini kullanır.
- Engine uçak prefabının kökünde, kendi Input Reader'ına bağlıdır.
- Debug panel hem ham komutu hem Engine throttle değerini gösterir. Engine olmayan eski prototiplerde durum N/A olarak gösterilir.
- Debug paneldeki klavye/fare etiketleri ayrı bir sabit tuş tablosundan değil, üretilen Input Action tanımlarının binding metadata'sından okunur.
- Debug panel yüksekliği yeni okuma satırına uyacak şekilde 350 px yapıldı; 320 px metin alanında hesaplanan 302,79 px içerik sığıyor.
- RPM, motor açma/kapama, thrust ve kuvvet uygulaması sonraki aşamadadır.

### Doğrulama

Play Mode'da kontrollü Input System girdileriyle yükselme, azalma, nötrde tutma, alt/üst sınır, yarım analog komut, devre dışı input, farklı başlangıç değeri ve yeniden etkinleştirme doğrulandı. 10/20/40 ms fizik adımlarında bir saniye tam komut aynı 0,5 throttle sonucunu verdi. Kontrol yüzeyi ve debug panel regresyon kontrolleri geçti; Console hata/uyarı sayısı sıfır. Bunlar kontrollü bileşen testleridir; fiziksel kontrol cihazıyla uçuş veya performans testi değildir.

---

## TD-024 — RPM Tabanlı Propulsion Prototipi ve Tekerlek Teması

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-15

### Karar

`AircraftEngine` motor durumunu, RPM geçişini ve itkiyi yönetir. Açık motorun hedef RPM'si throttle ile rölanti/maksimum arasında doğrusal hesaplanır; RPM hedefe saniyede belirlenen hızla yaklaşır. İtki, rölanti üzerindeki normalize RPM'nin karesi ile maksimum itkinin çarpımıdır. Rölantide itki sıfırdır. Motor kapalıyken itki sıfır olur, RPM zamanla sıfıra iner. Bileşen devre dışı bırakıldığında RPM ve itki çıkışları sıfırlanır.

Kuvvet `FixedUpdate` içinde root Rigidbody'nin fizik rotasyonuna göre modelin burun yönü olan yerel -Z ekseninde, kütle merkezine `ForceMode.Force` ile uygulanır. Kuvvet ayrıca delta time ile çarpılmaz. Kinematic Rigidbody'ye kuvvet uygulanmaz. `Rpm`, `NormalizedRpm` ve `ThrustNewtons` görsel/ses/UI tüketicilerine veri sağlar; bu tüketiciler motor durumunu sahiplenmez.

### Prototip değerleri ve sınırlar

Başlangıçta motor açık, throttle 0'dır. Rölanti 1.200 RPM, maksimum 6.000 RPM, geçiş hızı 3.000 RPM/s, maksimum itki 1.000 N'dir. Bu değerler Inspector'da ayarlanabilir; gerçek motor/propeller verileri olarak sunulmaz. Motor kapatmak frenleme sağlamaz. Drag modeli henüz olmadığı için sürekli itki altında nihai hız tanımlı değildir.

Üç tekerlek SphereCollider'ına `PM_AircraftWheelPrototype` atanır: static/dynamic friction 0, bounciness 0, combine Minimum. Primitive sphere'lar tüm uçak Rigidbody'sine bağlıdır; ayrı dönen tekerlek fiziği sağlamaz. Varsayılan sürtünme ile itki altında yaklaşık 14° eğilme ve çok az ilerleme gözlendi. Geçici düz zemin karşılaştırması sorunun temas sürtünmesinden kaynaklandığını doğruladı. 0,02 sürtünme tam gaz geçişinde yeterli olsa da gaz rampasında eğilmeyi gideremedi; final prototip değeri 0 seçildi. Bu temas çözümü yanal tutuş/fren sağlamaz; yönlü tutuş ve yuvarlanma direnci Ground Controller kapsamındadır. Collider boyutları, yerleşimleri ve Rigidbody ayarları korunur.

### Doğrulama

- İzole Unity fizik sahnesinde rölanti, RPM geçişi, kısmi/tam itki, kapatma, yeniden başlatma ve disabled input davranışı doğrulandı.
- 1.000 N / 100 kg, döndürülmüş gövdede bir saniyede 10 m/s hız üretti; 10/20/40 ms fizik adımlarında aynı sonuç alındı. 200 kg gövdede sonuç 5 m/s oldu. Yapay dönme torku oluşmadı.
- FlightTest'te 2026-09-15 tarihinde ölçülen yaklaşık 15,3 m ilerleme, daha sonra yanlış olduğu belirlenen +Z eksenindeydi. İleri eksen TD-025 ile -Z olarak düzeltildi; bu eski ölçüm yalnızca motor/rampa büyüklüğü kanıtı olarak tarihsel kayıttır.
- Testler kontrollü Input System olayları ve fizik simülasyonu kullanır; fiziksel kontrol cihazıyla kullanıcı uçuşu, performans veya nihai hız testi değildir.
- Debug panelde motor/RPM/itki okumaları için 450 px panel / 420 px metin alanı kullanılır; hesaplanan 385,59 px içerik sığar.

---

## TD-025 — Özel UAV İleri Ekseni Düzeltmesi

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-16

### Karar

Özel UAV'nin ileri yönü root yerel `-Z` eksenidir. `NoseGear_Tire` root uzayında yaklaşık `z = -1,70`, pusher `Rotor_Pivot` ise arkada `z = +2,40` konumundadır. Önceki `+Z` kabulü düzeltilmiştir. Engine itkiyi `Rigidbody.rotation * Vector3.back` yönünde uygular. FlightTest uçak yönü kullanıcı düzenlemesine uygun olarak `Y = 90°` kaydedilmiştir.

### Sonuçlar

Motor kuvveti görsel burunla aynı yöndedir. Kamera ve gelecekteki yön/heading hesapları da model ileri ekseni olarak `-Z` kullanmalıdır.

Kontrollü -Z yön testi üç saniyede yaklaşık 14,8 m ileri hareket ve 15,9 m/s hız üretti. FlightTest'in bu yönündeki zemin/tekerlek temasında yaklaşık 14° eğilme görüldü. Yön düzeltmesi doğrulandı; eğilme Ground Controller ve pist teması kapsamında açık teknik borçtur.

---

## TD-026 — Temel Takip Kamerası

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-16

### Karar

`AircraftFollowCamera`, kamera davranışını uçuş fiziğinden bağımsız bir `CameraSystem` bileşeninde tutar. Bileşen sahnedeki `Main Camera` üzerinde yaşar ve aktif `AircraftRoot` transformunu açık serialized referansla takip eder. Özel modelin ileri yönü yerel -Z olduğundan kamera ofseti yerel `(0, 3, 9)` seçilmiştir; böylece kamera gerçek arka tarafta 9 m geride ve 3 m yukarıda konumlanır. Bakış noktası yerel `(0, 0.8, -0.5)` konumudur.

Kamera oyun başlangıcında hedef pozuna doğrudan yerleşir. Sonraki karelerde konumu 0,15 saniyelik `SmoothDamp`, yönü saniyeden bağımsız üstel yumuşatma ile `LateUpdate` içinde güncellenir. Bakış rotasyonu dünya yukarı yönünü kullanır; uçak roll hareketi kamera ufkunu doğrudan döndürmez. Temel takip için ek paket bağımlılığı eklenmemiştir. `CustomUAVIntegrationTool`, sahneye yeniden uçak yerleştirirken mevcut kamera hedefini de yeni aktif instance'a bağlar.

### Doğrulama ve sınırlar

- Play Mode başlangıcında kamera ile hesaplanan hedef konum arasındaki hata 0,00002 m altında, bakış doğrultusu nokta çarpımı 1,0 olarak ölçüldü.
- Kontrollü 5 m hedef hareketinden sonra kamera hedef ofsetine yakınsadı ve bakış doğrultusunu korudu.
- 1280×720 Game View kontrolünde uçak kadrajın alt merkezinde, pist görüşü açık şekilde görüntülendi.
- Console hata ve uyarı sayısı sıfırdı.
- Geometri çarpışması, görüş engeli çözümü, kamera modları ve kullanıcı tercihli kalibrasyon bu temel bileşenin kapsamı dışındadır.

---

## TD-027 — RPM Tabanlı Pervane Görsel Animasyonu

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-16

### Karar

`AircraftPropellerAnimator`, uçak prefabının kökünde ayrı bir görsel tüketici olarak yaşar. Bileşen motor durumunu veya RPM üretimini sahiplenmez; yalnızca aynı kökteki `AircraftEngine.Rpm` çıktısını okur ve model altındaki `Rotor_Pivot` nesnesine yerel Y ekseni rotasyonu uygular. Böylece kontrol yüzeyi animasyonu, motor simülasyonu ve pervane görselleştirmesi birbirinden ayrılır.

RPM, dakikadaki devirden saniyedeki dereceye `RPM × 6` ile dönüştürülür. Görsel dönüş varsayılan olarak 0,1 ölçeğiyle uygulanır. Bu ölçek motorun RPM veya thrust hesabını değiştirmez; 1.200–6.000 RPM aralığının yaygın ekran yenileme hızlarında sabit ya da ters dönüyormuş gibi görünmesine yol açan zamansal alias etkisini azaltır. Fiziksel yüksek devir görünümü için blur/disc çözümü sonraki görsel iyileştirme kapsamındadır.

Bileşen devre dışı kaldığında rotor başlangıç yerel rotasyonuna döner. Motor devri sıfıra indiğinde pervane son açısında durur. Prefab doğrulaması tek kök `AircraftPropellerAnimator`, aynı kök Engine ve model altındaki doğru `Rotor_Pivot` referanslarını zorunlu tutar.

### Doğrulama

- 20 ms kare adımında 1.200 RPM için 14,4°, 6.000 RPM için 72° görsel dönüş ölçüldü; sonuçlar formülle eşleşti.
- Motorun 20 fizik adımında 0'dan yaklaşık 1.200 RPM'ye çıkışı rotor açısına kademeli olarak yansıdı.
- Motor durdurulup RPM sıfıra indikten sonra rotor açısı ek karede değişmedi.
- `PF_CustomUAVAircraftPrototype` ve bağlı `FlightTest` instance'ında tek bileşen ile Engine/Rotor referansları doğrulandı.
- Console hata ve uyarı sayısı sıfırdı.

---

## TD-028 — Motor Toggle Komutu ve Kademeli Kapanış

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-16

### Karar

Input System `Aircraft` action mapine `ToggleEngine` button actionı eklendi. Klavye bindingi `I` (Ignition), genel gamepad bindingi batı yüz düğmesidir. Üretilen `AircraftInputActions.cs` dosyası Input Action importer tarafından yeniden oluşturulur ve elle düzenlenmez.

`AircraftInputReader`, tuş basımını `EngineToggleRequested` olayı olarak yayınlar ve motor durumunu sahiplenmez. Aynı kökteki `AircraftEngine` bu olayı tüketerek `IsRunning` durumunu değiştirir. Böylece input yalnızca komut, Engine ise motor state sahibi olmaya devam eder.

Motor kapatıldığında thrust anında sıfırlanır. RPM, normal çalışma geçiş hızından ayrı olan varsayılan 1.200 RPM/s kapanış hızıyla sıfıra yaklaşır. Bu değer rölantiden yaklaşık 1 saniye, prototip maksimum 6.000 RPM'den yaklaşık 5 saniye duruş süresi üretir. `AircraftPropellerAnimator` aynı RPM çıktısını kullandığından pervane de kademeli yavaşlayarak son açısında durur.

Input Debug paneli `Engine Toggle [I]`, motor durumu ve RPM satırlarını birlikte gösterir. Tuş etiketi diğer komutlar gibi Input Action binding metadata'sından üretilir.

### Doğrulama

- Sanal klavye ile ilk `I` basımı çalışan motoru kapattı; ikinci basım yeniden çalıştırdı.
- Motor kapanışında thrust aynı anda sıfırlandı.
- 1.200 RPM rölantiden 0,98 saniye sonra yaklaşık 24 RPM, 1,00 saniye sonra 0 RPM ölçüldü.
- RPM sıfıra indikten sonraki fizik/görsel adımda rotor açısı değişmedi.
- Debug panel motor toggle bindingini ve `Stopped` durumunu gösterdi; 413,18 px tercih edilen içerik yüksekliği 420 px metin alanına sığdı.
- Console hata ve uyarı sayısı sıfırdı.

---

## TD-029 — Geçici Model İnceleme Sahnesinin Kaldırılması

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-16

### Karar

Model importu ve ilk görsel inceleme için kullanılan `AssetReview` sahnesi kaldırıldı. Sahne build listesinde bulunmuyordu ve kod, prefab veya runtime akışında referansı yoktu. Özel UAV artık üretim prefabı `PF_CustomUAVAircraftPrototype` ve ana geliştirme sahnesi `FlightTest` üzerinde doğrulandığından ayrı inceleme sahnesi yinelenen bakım yükü oluşturuyordu.

Model ölçeği, eksenleri, hareketli yüzeyleri, pervane pivotu, materyalleri, Rigidbody ve collider doğrulamaları bundan sonra üretim prefabı ile `FlightTest` üzerinde yürütülür. Silinen sahnenin önceki sürümleri Git geçmişinden geri alınabilir.

### Doğrulama

- Build listesinde yalnızca etkin `FlightTest` sahnesi bulunuyor.
- Silinen sahneye kod veya runtime referansı bulunmadığı doğrulandı.
- Sahne ve `.meta` dosyası Unity Asset Database üzerinden birlikte kaldırıldı.
- `FlightTest` yeniden açıldı; aktif, temiz ve kaydedilmiş durumda.

---

## TD-030 — Temel Aerodinamik Kuvvet ve Kontrol Prototipi

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-16

### Karar

`AircraftPhysics`, özel UAV prefabının kökünde `AircraftInputReader` ve tek `Rigidbody` ile birlikte yaşar. Bileşen sabit fizik adımında root yerel `-Z` ileri eksenini kullanarak hava hızını hesaplar; lift, drag ve pitch/roll/yaw torklarını aynı Rigidbody'ye `ForceMode.Force` ile uygular. Throttle, RPM, propulsion, görsel yüzey sapması ve yer hareketi bu bileşenin sorumluluğunda değildir.

Lift yalnızca pozitif ileri hızdan üretilen dinamik basınca göre uçak yerel yukarı yönünde uygulanır. Drag toplam hızın ters yönündedir. Kontrol torkları 5 m/s altında sıfırdır, 5–15 m/s arasında doğrusal artar ve 15 m/s üzerinde tam etkinliğe ulaşır. Varsayılan prototip değerleri 1,225 kg/m³ hava yoğunluğu, 10 m² kanat alanı, 1,0 lift katsayısı, 0,08 drag katsayısı ve pitch/roll/yaw için sırasıyla 250/500/300 N·m maksimum torktur. Değerler Inspector üzerinden düzenlenebilir ve gerçek uçak verisi olarak sunulmaz.

### Doğrulama ve sınırlar

- Sıfır hız, ileri/geri hız, lift/drag yönü, düşük/tam kontrol etkinliği, pitch/roll/yaw torkları ve devre dışı input davranışı izole Unity fizik sahnesinde doğrulandı.
- Bir saniyelik aerodinamik entegrasyon 10, 20 ve 40 ms fizik adımlarında yaklaşık 0,1 m/s azami hız farkı üretti.
- FlightTest pistinde üç saniyelik kontrollü tam gaz testinde yaklaşık 14,8 m ileri hareket, 16,6 m/s hız, 1.452 N lift ve 131 N drag ölçüldü. Uçak yaklaşık 0,49 m yükselirken mevcut zemin/tekerlek sistemiyle yaklaşık 15,4° tepe açı değişimi oluştu.
- Unity Console hata/uyarı vermedi; Play Mode sonrasında `FlightTest` temiz ve kaydedilmiş kaldı.
- Mevcut model sabit lift katsayılı ilk prototiptir. Açı-of-attack, stall, indüklenmiş drag, rüzgâr, irtifa yoğunluğu ve maksimum güvenli hız davranışı sonraki aerodinamik kalibrasyon kapsamındadır.
- Pistte yönlü tutuş, yuvarlanma direnci ve fren davranışı `AircraftGroundController` kapsamındadır.

---

## TD-031 — Kuvvet Tabanlı Yer Hareketi ve Fren Prototipi

**Durum:** Kabul edildi ve uygulandı

**Tarih:** 2026-09-16

### Karar

`AircraftGroundController`, özel UAV prefabının kökünde `AircraftInputReader` ve tek `Rigidbody` ile birlikte yaşar. Yeni Rigidbody, WheelCollider veya temas colliderı eklemez. Mevcut `NoseWheelCollider`, `RightMainWheelCollider` ve `LeftMainWheelCollider` SphereCollider bileşenlerini aşağı yönlü yer probları olarak kullanır; kendi compound collider gövdesini sorgu sonuçlarından çıkarır.

Yer hareketi sabit fizik adımında Rigidbody üzerinde uygulanır. Yanal hız, saniyede 8 tepki oranlı yönlü tutuşla sönümlenir. Yuvarlanma direnci 0,8 m/s², `Space` fren komutu 12 m/s² prototip yavaşlama uygular ve hız yönünü tersine çevirmeden sıfıra yaklaşır. Burun tekeri temastayken yaw komutu 1–6 m/s arasında artan, 20 m/s'ye kadar azalan düşük hızlı pist yönlendirmesi üretir. Yer yaw sönümlemesi kontrolsüz heading sapmasını azaltır.

Pist hizalama desteği düşük hızda gövde up eksenini zemin normaline yaklaştırır ve pitch/roll açısal hızını sönümler. Destek 8 m/s'ye kadar tamdır, 15 m/s'de sıfıra iner; böylece kalkış aşamasında aerodinamik kontrolü devralmak üzere bırakılır. Üç teker için kullanılan 0,2 m prob payı ilk yer hareketi prototipidir ve gerçek süspansiyon geometrisi olarak yorumlanmaz.

Tekerleklerin sıfır sürtünmeli fizik materyali korunmuştur. Yönlü tutuş ve fren davranışı Ground Controller tarafından açık ve test edilebilir kuvvetlerle sağlanır. Dönen tekerlek, süspansiyon, gelişmiş lastik modeli ve iniş davranışı sonraki kapsamdır.

### Doğrulama ve sınırlar

- İzole Unity fizik sahnesinde üç teker temas algısı ve hareketsiz pist stabilitesi doğrulandı.
- 5 m/s başlangıç yanal hızı bir saniyede yaklaşık 0,001 m/s'ye düştü.
- 8 m/s başlangıç ileri hızı 0,5 saniyede serbest yuvarlanmada 7,45 m/s, frenlemede 1,53 m/s ölçüldü.
- İzole üç saniyelik tam gaz koşusu yaklaşık 10,49 m ilerleme, 13,18 m/s hız ve 0° tepe açı değişimi üretti.
- FlightTest pistindeki kontrollü üç saniyelik koşu yaklaşık 12,35 m ilerleme, 14,14 m/s hız, 1.197 N lift ve 96 N drag üretti. Üç teker probu temasını korudu; tepe açı değişimi 0° ve yanal hız yaklaşık 0,00006 m/s ölçüldü.
- Unity Console hata/uyarı vermedi. `FlightTest` üzerinde Ground Controller kaynaklı prefab override veya kaydedilmemiş sahne değişikliği oluşmadı.
- Bu test kalkış hızı veya tam uçuş kabulü değildir. Kalkış rotasyonu, liftoff eşiği, pist dışı davranış, iniş ve gerçek tekerlek/süspansiyon kalibrasyonu sonraki görevlerdir.

---

## TD-032 — Kontrollü kalkış referansı ve kabul senaryosu

**Durum:** Kabul edildi — 2026-09-16

### Karar

Mevcut prototip için kontrollü kalkış test akışı; motor çalışırken tam gaz hızlanma, 13 m/s ileri hava hızında pozitif pitch (`S`) komutu ve nötr roll/yaw girdileri olarak tanımlanmıştır. Yerden kesilme kabulü, üç teker probunun en az beş ardışık sabit fizik adımında temassız kalması ve dikey hızın pozitif olmasıdır.

`InputDebugPanel`, bu akışın kullanıcı tarafından tekrarlanabilmesi için `AircraftPhysics.ForwardAirspeed` değerini ve `AircraftGroundController` teker temas durumunu gösterir. Panel referansları aktif uçaktaki Input Reader üzerinden çalışma zamanında çözümlenir; uçak prefabına veya sahneye yeni fizik bileşeni eklenmez.

### Doğrulama

- İzole yerel fizik sahnesinde ve açık `FlightTest` pistinde aynı kontrollü senaryo çalıştırıldı.
- Rotasyon komutu yaklaşık 3,00 saniye ve 10,49 m pist mesafesinde verildi.
- Burun tekeri yaklaşık 3,32 saniye ve 15,59 m/s ileri hava hızında yerden ayrıldı.
- Tüm tekerler yaklaşık 3,54 saniye, 17,18 m/s ileri hava hızı ve 18,94 m pist mesafesinde teması bıraktı.
- Ölçüm anında dikey hız yaklaşık 2,71 m/s, tepe pitch yaklaşık 11°, mutlak roll sapması 0° ve pist orta hattı sapması 0,002 m altında kaldı.
- `Left Shift` ve `S` Input System bindingleri sanal klavye üzerinden doğrulandı. Unity Console hata veya uyarı vermedi; sahne Play Mode sonrasında temiz kaldı.

### Sınırlar

13 m/s rotasyon ve yaklaşık 17,2 m/s yerden kesilme değerleri gerçek araç performansı değil, mevcut 100 kg / 10 m² / sabit `C_L = 1` prototipinin test referanslarıdır. Pitch komutu verilmeyen karşılaştırma koşusunda uçak yaklaşık 17,12 m/s ve 18,15 m pist mesafesinde kendiliğinden yerden kesilmiştir. Bu sonuç sabit lift katsayısının açı-of-attack bağımsız olmasından kaynaklanır; sonraki görev açı-of-attack, stall ve maksimum güvenli hız modelini uygulayacak ve kalkış referanslarını yeniden kalibre edecektir.

---

## Karar Bekleyen Konular

- [ ] Yakıt sistemi veya batarya sistemi
- [ ] Harita çözümü
- [ ] Test framework kapsamı
- [ ] Cinemachine kullanımı
- [ ] LOD kapsamı
- [ ] Gerçek araç kütlesi, ağırlık merkezi ve inertia verileri

Joystick/HOTAS desteği MVP sonrasına ertelenmiştir; yeniden değerlendirilene kadar karar bekleyen MVP maddesi değildir.

---

## Değişiklik Günlüğü

| Tarih | Karar | Değişiklik |
|---|---|---|
| Proje başlangıcı | TD-001 – TD-016 | İlk teknik karar taslağı oluşturuldu |
| 2026-09-11 | TD-009, TD-010 | URP ve uGUI + TextMeshPro kararları kabul edildi olarak güncellendi |
| 2026-09-11 | TD-017 – TD-020 | Unity temeli, özel model, üçüncü taraf asset ve kuvvet sorumlulukları belgelendi |
| 2026-09-12 | TD-018, TD-021 | Özel UAV Unity entegrasyonu, compound collider ve görsel kontrol yüzeyi sınırları doğrulandı |
| 2026-09-15 | TD-022 | Input Reader ve görsel Animator prefab sahipliği uygulandı |
| 2026-09-15 | TD-023 | Throttle state/ramp Engine'e taşındı; sabit zaman adımı ve input ayrımı doğrulandı |
| 2026-09-15 | TD-024 | RPM/itki prototipi ve tekerlek temas materyali uygulandı; kısa pist hızlanması doğrulandı |
| 2026-09-16 | TD-025 | Özel UAV ileri ekseni -Z olarak düzeltildi; sahne yönü Y=90° kaydedildi |
| 2026-09-16 | TD-026 | Temel yumuşak takip kamerası aktif uçağa bağlandı ve Play Mode'da doğrulandı |
| 2026-09-16 | TD-027 | Pervane görsel dönüşü motor RPM verisine bağlandı ve prefab üzerinde doğrulandı |
| 2026-09-16 | TD-028 | Motor toggle inputu ve kademeli RPM/pervane kapanışı uygulandı |
| 2026-09-16 | TD-029 | Geçici model inceleme sahnesi kaldırıldı; doğrulama akışı FlightTest'te birleştirildi |
| 2026-09-16 | TD-030 | Temel lift, drag ve hıza bağlı pitch/roll/yaw torkları sabit fizik adımında uygulandı |
| 2026-09-16 | TD-031 | Üç teker temasına dayalı yönlü yer tutuşu, pist stabilizasyonu, yönlendirme ve fren prototipi uygulandı |
| 2026-09-16 | TD-032 | 13 m/s rotasyon komutlu kontrollü kalkış senaryosu ve yaklaşık 17,2 m/s yerden kesilme referansı doğrulandı |
