# UAV Flight Simulator

Unity ve C# kullanılarak geliştirilen, sabit kanatlı bir insansız hava aracının kalkış, görev uçuşu, hedef gözlemi ve iniş süreçlerini simüle eden masaüstü portföy projesidir.

> Bu proje bireysel bir yazılım ve simülasyon çalışmasıdır. Resmî bir Baykar ürünü değildir ve Baykar tarafından desteklendiği veya onaylandığı iddiasını taşımaz.

---

## Proje Hakkında

UAV Flight Simulator, bir yer kontrol istasyonu arayüzü üzerinden İHA uçuşunun yönetilmesini amaçlamaktadır.

Kullanıcı:

- İHA sistemlerini hazırlayabilecek
- Pistten kalkış yapabilecek
- Belirlenen waypoint rotasını takip edebilecek
- Elektro-optik kamera ile hedef bölgesini gözlemleyebilecek
- Görev hedefini tamamlayabilecek
- Üs bölgesine dönerek iniş yapabilecek
- Uçuş boyunca telemetri verilerini izleyebilecek

Proje, Baykar iş başvurusunda teknik portföy çalışması olarak sunulmak üzere geliştirilmektedir.

---

## Proje Durumu

**Mevcut aşama:** Test ortamı ve havaalanı prototipi

| Sistem | Durum |
|---|---|
| Proje dokümantasyonu | Tamamlandı |
| Unity proje kurulumu | Tamamlandı |
| İHA uçuş fiziği | Planlandı |
| Kamera sistemi | Planlandı |
| Telemetri sistemi | Planlandı |
| Görev sistemi | Planlandı |
| Yer kontrol istasyonu UI | Planlandı |
| Windows build | Planlandı |

---

## Planlanan Özellikler

### Uçuş sistemi

- Rigidbody tabanlı uçuş fiziği
- Motor gücü ve throttle kontrolü
- Pitch, roll ve yaw kontrolü
- Lift ve drag kuvvetleri
- Basitleştirilmiş stall davranışı
- Pist üzerinde hareket
- Kalkış ve iniş

### Kamera sistemi

- Takip kamerası
- Gövde kamerası
- Serbest kamera
- Elektro-optik hedefleme kamerası
- Zoom ve hedef takibi

### Görev sistemi

- Görev brifingi
- Waypoint rotası
- Hedef bölgesi
- Görev başarı ve başarısızlık koşulları
- Üsse dönüş
- Görev sonuç ekranı

### Telemetri ve arayüz

- Hız
- İrtifa
- Dikey hız
- Heading
- Pitch, roll ve yaw
- Throttle
- Waypoint mesafesi
- Kamera modu
- Görev durumu
- Sistem uyarıları

---

## MVP Kapsamı

İlk tamamlanabilir sürüm aşağıdakileri içerecektir:

1. Tek bir sabit kanatlı İHA
2. Tek bir havaalanı veya test sahası
3. Klavye ve fare kontrolü
4. Temel fizik tabanlı uçuş
5. Kalkış ve iniş
6. Takip kamerası
7. Elektro-optik kamera
8. Temel telemetri paneli
9. En az üç waypoint içeren bir görev
10. Windows çalıştırılabilir build

---

## Kullanılan Teknolojiler

| Teknoloji | Kullanım amacı |
|---|---|
| Unity 6.3 LTS — 6000.3.20f1 | Simülasyon ve oyun motoru |
| C# | Uçuş, görev ve arayüz sistemleri |
| Universal Render Pipeline | Windows için dengeli görsel kalite ve performans |
| Unity Input System | Klavye, fare ve gelecekteki kontrolcü girdileri |
| Unity Physics | Rigidbody tabanlı uçuş ve çarpışma |
| uGUI ve TextMeshPro | Telemetri ve yer kontrol istasyonu arayüzü |
| Git | Sürüm kontrolü |
| GitHub | Kaynak kod ve portföy sunumu |

### Kesinleştirilecek teknolojiler

- Cinemachine kullanımı
- Joystick desteği

---

## Teknik Yaklaşım

Proje, tek bir büyük kontrol sınıfı yerine sorumlulukları ayrılmış modüler bileşenlerden oluşacaktır.

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

### Temel prensipler

- Girdi okuma ve fizik uygulaması ayrılacaktır.
- Fizik işlemleri `FixedUpdate` içinde çalıştırılacaktır.
- Ayarlar mümkün olduğunca Inspector üzerinden düzenlenebilir olacaktır.
- Büyük özellikler ayrı Git branchlerinde geliştirilecektir.
- Ana mekanikler hazır bir uçuş sistemi paketine teslim edilmeyecektir.
- Kod okunabilirlik ve genişletilebilirlik gözetilerek yazılacaktır.

Daha ayrıntılı kararlar için [`TECHNICAL_DECISIONS.md`](TECHNICAL_DECISIONS.md) dosyasına bakılabilir.

---

## Planlanan Klasör Yapısı

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

## Kontroller

Kontrol şeması geliştirme aşamasında kesinleştirilecektir.

Planlanan varsayılan kontroller:

| İşlem | Tuş |
|---|---|
| Pitch | W / S |
| Roll | A / D |
| Yaw | Q / E |
| Throttle artır | Left Shift |
| Throttle azalt | Left Control |
| Fren | Space |
| Kamera değiştir | C |
| EO kamera zoom | Mouse Wheel |
| Pause | Escape |

Bu tuşlar proje geliştirilirken değişebilir.

---

## Kurulum

Proje kaynak kodu henüz yayınlanabilir bir geliştirme aşamasına ulaşmamıştır.

İleride temel geliştirme kurulumu aşağıdaki şekilde olacaktır:

1. Uyumlu Unity Hub sürümünü yükleyin.
2. Repoyu klonlayın.
3. Projeyi belirtilen Unity sürümüyle açın.
4. Unity'nin paketleri içe aktarmasını bekleyin.
5. Ana sahneyi açın.
6. Play düğmesine basın.

```bash
git clone <repository-url>
```

Kesin Unity sürümü belirlendiğinde bu bölüm güncellenecektir.

---

## Build Çalıştırma

Windows build yayımlandığında:

1. Sürüm arşivini indirin.
2. ZIP dosyasını bir klasöre çıkarın.
3. Uygulamanın `.exe` dosyasını çalıştırın.
4. Kontroller ekranını inceleyin.
5. Simülasyonu başlatın.

---

## Dokümantasyon

| Dosya | Açıklama |
|---|---|
| [`PROJECT_OVERVIEW.md`](PROJECT_OVERVIEW.md) | Projenin amacı, kapsamı ve başarı kriterleri |
| [`TECHNICAL_DECISIONS.md`](TECHNICAL_DECISIONS.md) | Alınan teknik kararlar ve gerekçeleri |
| [`TASKS.md`](TASKS.md) | Geliştirme aşamaları ve görev takibi |
| [`README.md`](README.md) | GitHub ve portföy tanıtımı |

---

## Ekran Görüntüleri

Projenin görsel geliştirmesi başladığında bu bölüme eklenecektir.

```text
docs/images/
├── main-menu.png
├── takeoff.png
├── flight.png
├── eo-camera.png
└── landing.png
```

Örnek kullanım:

```markdown
![Takeoff](docs/images/takeoff.png)
```

---

## Geliştirme Yol Haritası

- [x] Proje fikrinin belirlenmesi
- [x] Başlangıç dokümantasyonunun hazırlanması
- [ ] Unity projesinin oluşturulması
- [ ] Test havaalanının hazırlanması
- [ ] İHA modelinin eklenmesi
- [ ] Girdi sisteminin geliştirilmesi
- [ ] Temel uçuş fiziğinin geliştirilmesi
- [ ] Kalkış ve iniş sisteminin geliştirilmesi
- [ ] Kamera sisteminin geliştirilmesi
- [ ] Telemetri arayüzünün geliştirilmesi
- [ ] Waypoint ve görev sisteminin geliştirilmesi
- [ ] Ses ve görsel iyileştirmeler
- [ ] Optimizasyon
- [ ] Windows build
- [ ] Tanıtım videosu ve portföy sunumu

Ayrıntılı görev listesi için [`TASKS.md`](TASKS.md) dosyasına bakılabilir.

---

## Bilinen Eksikler

Proje henüz geliştirme başlangıcındadır. Bu nedenle şu sistemler mevcut değildir:

- Çalışan uçuş fiziği
- Tamamlanmış İHA modeli entegrasyonu
- Yer kontrol istasyonu arayüzü
- Waypoint görevi
- EO kamera hedefleme sistemi
- Windows build

Bu bölüm geliştirme süresince düzenli olarak güncellenecektir.

---

## Gelecek Geliştirmeler

MVP tamamlandıktan sonra değerlendirilebilecek özellikler:

- Joystick ve HOTAS desteği
- Rüzgâr ve türbülans sistemi
- Farklı hava koşulları
- Gece uçuşu
- İniş takımı animasyonu
- Yakıt veya enerji yönetimi
- Otomatik pilot
- Gelişmiş harita sistemi
- Birden fazla görev
- Yeniden oynatma sistemi
- Uçuş veri kaydı
- Yapay zekâ destekli hedef davranışları

---

## Lisans ve Üçüncü Taraf İçerikler

Kaynak kod lisansı proje yayımlanmadan önce belirlenecektir.

Üçüncü taraf model, ses, doku ve paketler kendi lisanslarına tabidir. Ücretli veya yeniden dağıtımı yasak olan asset dosyaları açık kaynak depoya eklenmeyecektir.

---

## Geliştirici

**Mert Kaan**  
Yazılım Mühendisi / Unity Geliştiricisi

Bu proje, yazılım geliştirme, Unity, C#, fizik tabanlı sistemler ve teknik dokümantasyon yetkinliklerini göstermek amacıyla geliştirilmektedir.
