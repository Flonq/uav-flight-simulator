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

**Son güncelleme:** 17 Eylül 2026

**Mevcut aşama:** Açı-of-attack, stall, sideslip ve prototip hız zarfı doğrulandı; sıradaki adım kullanıcı kabulü ve iniş/yer davranışı geliştirmesidir

| Sistem | Durum |
|---|---|
| Unity 6.3 LTS / URP proje kurulumu | Tamamlandı |
| Test havaalanı ve çevre | Çalışır prototip |
| Rigidbody fizik kökü ve 10 parçalı compound collider | Tamamlandı ve Play Mode'da doğrulandı |
| Input System — klavye ve gamepad | Tamamlandı; aktif uçak/debug bağlantısı ve motor toggle komutu doğrulandı |
| Özel Blender İHA modeli | Unity importu, URP materyali ve prefab entegrasyonu tamamlandı |
| Görsel kontrol yüzeyi animasyonları | Tamamlandı ve Play Mode'da doğrulandı |
| Motor, RPM ve itki | Sabit fizik adımında çalışan prototip; görsel burun yönü ve pervane animasyonuyla eşleştirildi |
| Aerodinamik uçuş fiziği | Toplam hava hızı, signed AoA, stall drag, sideslip toparlanması ve 75/85/80 m/s prototip hız zarfı sabit fizik adımında çalışıyor; gerçek araç verisiyle kalibrasyon açık |
| Yer hareketi ve fren | Üç teker temas kontrolü, yanal tutuş, yuvarlanma direnci, düşük hızlı yönlendirme ve fren tamamlandı |
| Kontrollü kalkış | 13 m/s rotasyon komutu ve yaklaşık 17,2 m/s yerden kesilme referansı FlightTest pistinde doğrulandı |
| Kamera ve EO sistemi | Temel takip kamerası tamamlandı; diğer modlar planlandı |
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
- AoA tabanlı basitleştirilmiş stall ve post-stall davranışı
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
| Motor aç/kapat | I |
| Fren | Space |
| Kamera değiştir | C |
| EO kamera zoom | Mouse Wheel |
| Pause | Escape |

DualSense, genel `<Gamepad>` bindingleri üzerinden test edilmiştir. Joystick/HOTAS MVP sonrasına ertelenmiştir.

### Kontrollü kalkış prototipi

1. Game View'a odaklanın ve gerekirse `I` ile motoru çalıştırın.
2. `Left Shift` tuşunu basılı tutarak tam gaz hızlanın.
3. Debug panelde `Forward Airspeed` 13 m/s değerine ulaştığında `S` tuşunu basılı tutarak burun yukarı komutu verin.
4. Roll ve yaw komutlarını nötr tutun. Yeni prototipte burun tekeri ve tüm teker temas geçişleri uçuş durumuna göre değişebilir; debug panelde Angle of Attack, Flight State ve Vertical Speed değerlerini izleyin.

Önceki yaklaşık 17,2 m/s değeri sabit lift katsayılı modelin tarihsel test referansıdır ve yeni modelin kabul kriteri değildir. Yeni prototipte toplam hava hızı, açı-of-attack, stall davranışı ve dikey hız birlikte değerlendirilmelidir.

### Prototip hız zarfı

Debug panelindeki `Speed State`, total air-relative airspeed üzerinden değerlendirilir:

- `Normal`: 75 m/s altında
- `Caution`: 75 m/s ve üzerinde
- `Overspeed`: 85 m/s ve üzerinde
- Overspeed recovery: hız 80 m/s veya altına indiğinde

Bu eşikler hızı doğrudan kesmez. Tam gaz longitudinal hız, mevcut thrust ve drag kuvvetlerinin doğal dengesiyle yaklaşık 65,7–66,3 m/s aralığına yaklaşır. Eşikler prototip değerleridir ve gerçek araç Vne/işletme limitleri olarak yorumlanmamalıdır.

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
- [x] Motor durumu, RPM geçişi ve ileri yön itki prototipi
- [x] `Rotor_Pivot` görsel dönüşünün motor RPM verisine bağlanması
- [x] Motor ve throttle sisteminin geliştirilmesi
- [~] Temel uçuş fiziğinin geliştirilmesi — AoA/stall, rate feedback, sideslip ve prototip hız zarfı tamamlandı; gerçek veri kalibrasyonu açık
- [x] Temel yer hareketi, pist stabilizasyonu ve fren prototipi
- [~] Kalkış ve iniş sisteminin geliştirilmesi — kontrollü kalkış referansı doğrulandı; iniş ve nihai kalibrasyon planlandı
- [~] Kamera sisteminin geliştirilmesi — temel yumuşak takip kamerası tamamlandı
- [ ] Telemetri arayüzünün geliştirilmesi
- [ ] Waypoint ve görev sisteminin geliştirilmesi
- [ ] Ses ve görsel iyileştirmeler
- [ ] Optimizasyon
- [ ] Windows build
- [ ] Tanıtım videosu ve portföy sunumu

Ayrıntılı görev listesi için [`TASKS.md`](TASKS.md) dosyasına bakılabilir.

---

## Bilinen Eksikler

2026-09-15: Input Reader, Engine ve kontrol yüzeyi Animator'ı uçak prefabının kökündedir. Debug panel gaz komutunu, throttle, motor durumunu, RPM, itki, ileri hava hızı ve teker temas durumunu gösterir. Motor prototipi 1.200 rölanti / 6.000 maksimum RPM, 3.000 RPM/s geçiş ve 1.000 N maksimum itki kullanır. Bu değerler gerçek araç verileri değildir.

2026-09-16: Modelin burun yönü `-Z` olarak düzeltildi. `AircraftGroundController`, üç mevcut tekerlek collider'ından yer temasını okur; yanal tutuş, yuvarlanma direnci, düşük hızlı yaw yönlendirmesi, pist stabilizasyonu ve fren uygular. Kontrollü FlightTest koşusunda önceki yaklaşık 15° yön değişimi ölçülmedi.

2026-09-16: Kontrollü kalkış senaryosunda 13 m/s'de `S` rotasyon komutu verildi. Bu ölçüm, sabit lift katsayılı eski modelin tarihsel referansıdır.
2026-09-17: Yeni uçuş modelinde nötr pitch koşulunda azami pitch değişimi 0,0098 derece ve yükseklik değişimi 0,0024 m ölçüldü. Kontrollü kalkışta rotasyon hızı 13,17 m/s, liftoff hızı 21,46 m/s ve azami pitch 10,66 derece ölçüldü; pitch bırakıldıktan sonra açısal hız sönümlendi.
2026-09-17: Mevcut thrust/drag dengesi değiştirilmeden yaklaşık 65,71 m/s doğal longitudinal denge doğrulandı. Debug paneline total airspeed tabanlı `Normal`, `Caution` ve `Overspeed` durumu ile 85 m/s giriş eşiği eklendi; overspeed 80 m/s veya altında temizlenir.

Play Mode'da giriş komutları için Game View'a odaklanın. `I` tuşu motoru açıp kapatır, `Space` yerde fren uygular. `AircraftRoot > AircraftEngine` bileşen menüsündeki `Start Engine` / `Stop Engine` seçenekleri tanısal kullanım içindir. Motor kapatma itkiyi keser ancak otomatik fren uygulamaz. Kısa testten sonra Play Mode'dan çıkın.

Mevcut doğrulanmış eksikler:

- Gerçek kütle, ağırlık merkezi ve inertia değerlerinin fiziksel verilerle kalibre edilmesi
- Son sistem doğrulamaları bitince eski Meshy yedeği ve iki inactive uçak instance'ının temizlenmesi
- Motor/propeller verilerine dayalı itki kalibrasyonu ve hıza bağlı propeller verimi
- Prototip 75/85/80 m/s hız zarfının gerçek araç Vne ve işletme limitleriyle yeniden kalibre edilmesi
- Prototip drag katsayısıyla nihai yer/uçuş hızının ve kontrol torklarının kullanıcı uçuş testinde kalibre edilmesi
- Nihai kalkış hızlarının açı-of-attack/stall modeli ve gerçek araç verileriyle yeniden kalibre edilmesi
- Pist dışı davranış, iniş ve gelişmiş tekerlek/süspansiyon sistemi
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
