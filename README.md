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

**Son güncelleme:** 12 Eylül 2026

**Mevcut aşama:** Özel İHA entegrasyonu ve motorun throttle yönetimi tamamlandı; RPM, thrust ve propulsion geliştirmesi sırada

| Sistem | Durum |
|---|---|
| Unity 6.3 LTS / URP proje kurulumu | Tamamlandı |
| Test havaalanı ve çevre | Çalışır prototip |
| Rigidbody fizik kökü ve 10 parçalı compound collider | Tamamlandı ve Play Mode'da doğrulandı |
| Input System — klavye ve gamepad | Tamamlandı; aktif uçak/debug bağlantısı doğrulandı |
| Özel Blender İHA modeli | Unity importu, URP materyali ve prefab entegrasyonu tamamlandı |
| Görsel kontrol yüzeyi animasyonları | Tamamlandı ve Play Mode'da doğrulandı |
| Motor ve gerçek thrust | Başlanmadı |
| Aerodinamik uçuş fiziği | Başlanmadı |
| Kamera ve EO sistemi | Planlandı |
| Telemetri ve görev sistemi | Planlandı |
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
| Unity 6000.3.20f1 / Unity 6.3 LTS | Simülasyon ve oyun motoru |
| Universal Render Pipeline (URP) | Windows hedefli render altyapısı |
| C# | Uçuş, görev ve arayüz sistemleri |
| Unity Input System | Klavye, fare ve kontrolcü girdileri |
| Unity Physics | Rigidbody tabanlı uçuş ve çarpışma |
| uGUI + TextMeshPro | Debug, telemetri ve yer kontrol arayüzü |
| Blender 5.2.1 LTS | Özel İHA görsel modeli |
| Git | Sürüm kontrolü |
| GitHub | Kaynak kod ve portföy sunumu |

### Ertelenen veya değerlendirilecek teknolojiler

- Cinemachine kullanımı kamera fazında yeniden değerlendirilecek.
- Joystick/HOTAS desteği MVP sonrasına ertelendi.
- Harita çözümü henüz kesinleşmedi.

---

## Teknik Yaklaşım

Proje, tek bir büyük kontrol sınıfı yerine sorumlulukları ayrılmış modüler bileşenlerden oluşacaktır.

Planlanan temel bileşenler:

```text
AircraftInputReader
AircraftPhysics
AircraftEngine
AircraftControlSurfaceAnimator
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

Mevcut doğrulanmış klavye/fare kontrolleri:

| İşlem | Tuş |
|---|---|
| Pitch | W / S — W burun aşağı, S burun yukarı |
| Roll | A / D |
| Yaw | Q / E |
| Throttle artır | Left Shift |
| Throttle azalt | Left Control |
| Fren | Space |
| Kamera değiştir | C |
| EO kamera zoom | Mouse Wheel |
| Pause | Escape |

DualSense, genel `<Gamepad>` bindingleri üzerinden test edilmiştir. Joystick/HOTAS MVP sonrasına ertelenmiştir.

---

## Kurulum

Geliştirme ortamının doğrulanmış temeli:

1. Unity Hub üzerinden Unity `6000.3.20f1` sürümünü yükleyin.
2. Repoyu klonlayın.
3. Yerel kullanım lisansına sahip Military Base Pack'i beklenen `Assets/ThirdParty/` yoluna kurun. Bu paket açık Git deposunda dağıtılmaz.
4. Projeyi belirtilen Unity sürümüyle açın.
5. Unity'nin paketleri içe aktarmasını bekleyin.
6. `Assets/_Project/Scenes/FlightTest.unity` sahnesini açın.
7. Console'un temiz olduğunu doğrulayın.

```bash
git clone https://github.com/Flonq/uav-flight-simulator.git
```

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

Paylaşılabilir güncel Unity ekran görüntüleri ve final özel İHA entegrasyonu tamamlandığında bu bölüm güncellenecektir.

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
- [x] Unity 6.3 LTS / URP projesinin oluşturulması
- [x] Test havaalanının hazırlanması
- [x] Rigidbody fizik kökü ve collider prototipinin hazırlanması
- [x] Girdi sisteminin geliştirilmesi
- [x] Özel İHA modelinin Blender'da tamamlanması
- [x] Özel İHA modelinin Unity'ye aktarılması ve doğrulanması
- [x] Compound colliderların ayarlanması ve yer temasının doğrulanması
- [x] Görsel kontrol yüzeyi animasyonlarının uygulanması
- [x] Throttle komutu ile motor durumunun ayrılması ve sabit zaman adımında gaz rampası
- [ ] Motor ve throttle sisteminin geliştirilmesi
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

2026-09-15: Input Reader, Engine ve kontrol yüzeyi Animator'ı uçak prefabının kökündedir. Engine, throttle değerini 0–1 aralığında saniyede 0,5 hızla yönetir; debug panel gaz komutunu ve motorun throttle değerini ayrı gösterir. RPM ve itki henüz uygulanmamıştır.

Mevcut doğrulanmış eksikler:

- Gerçek kütle, ağırlık merkezi ve inertia değerlerinin fiziksel verilerle kalibre edilmesi
- `AssetReview` sahnesinde final model smoke testinin kaydedilmesi
- Son sistem doğrulamaları bitince eski Meshy yedeği ve iki inactive uçak instance'ının temizlenmesi
- Gerçek motor thrust sistemi
- Lift, drag, stall ve aerodinamik kontrol kuvvetleri
- Tam yer hareketi, kalkış ve iniş sistemi
- Yer kontrol istasyonu arayüzü
- Waypoint görevi
- EO kamera hedefleme sistemi
- Telemetri sistemi
- Ses sistemi
- Windows build

Doğrulanmış özel İHA prefabı ve elle ayarlanmış colliderlar korunacaktır. Eski Meshy yedeği, yeni uçağın motor ve uçuş fiziği de doğrulanmadan silinmeyecektir.

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
- Dinamik hedef davranışları

---

## Lisans ve Üçüncü Taraf İçerikler

Kaynak kod lisansı proje yayımlanmadan önce belirlenecektir.

Üçüncü taraf model, ses, doku ve paketler kendi lisanslarına tabidir. Ücretli veya yeniden dağıtımı yasak olan asset dosyaları açık kaynak depoya eklenmeyecektir.

Test havaalanında kullanılan Tiny Teacup Studio Military Base Pack yerel geliştirme bağımlılığıdır ve ham dosyaları bu açık repository içinde dağıtılmaz. Özel İHA modeli proje için Blender'da sıfırdan geliştirilmiştir.

---

## Geliştirici

**Mert Kaan**  
Yazılım Mühendisi / Unity Geliştiricisi

Bu proje, yazılım geliştirme, Unity, C#, fizik tabanlı sistemler ve teknik dokümantasyon yetkinliklerini göstermek amacıyla geliştirilmektedir.
