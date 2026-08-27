# UAV Flight Simulator

Unity ve C# kullanılarak geliştirilen, sabit kanatlı bir insansız hava aracının kalkış, görev uçuşu, hedef gözlemi ve iniş süreçlerini simüle etmeyi amaçlayan masaüstü portföy projesidir.

> Bu proje bireysel bir yazılım ve simülasyon çalışmasıdır. Resmî bir Baykar ürünü değildir ve Baykar tarafından desteklendiği veya onaylandığı iddiasını taşımaz.

---

## Proje Hakkında

UAV Flight Simulator, bir yer kontrol istasyonu arayüzü üzerinden İHA uçuşunun yönetilmesini amaçlamaktadır.

Planlanan görev akışında kullanıcı:

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

**Mevcut aşama:** Faz 4 tamamlandı — Faz 5 Motor ve Throttle Sistemi geliştirmesine geçiliyor.

| Sistem | Durum |
|---|---|
| Proje dokümantasyonu ve temel teknik kararlar | Tamamlandı |
| Unity proje kurulumu | Tamamlandı |
| Test havaalanı ve üs ortamı | Tamamlandı |
| İHA model entegrasyonu ve fizik kökü | Tamamlandı |
| Girdi sistemi | Tamamlandı |
| Motor ve throttle sistemi | Sıradaki aşama |
| Temel uçuş fiziği | Planlandı |
| Kamera sistemi | Planlandı |
| Telemetri sistemi | Planlandı |
| Görev sistemi | Planlandı |
| Yer kontrol istasyonu UI | Planlandı |
| Windows build | Planlandı |

### Tamamlanan prototip altyapısı

- Unity 6.3 LTS projesi ve URP yapılandırması
- Unity Input System ve TextMeshPro altyapısı
- Modüler `_Project` klasör yapısı
- `FlightTest` ana test sahnesi
- Military Base Pack tabanlı test havaalanı ve üs çevresi
- URP uyumlu environment materyalleri
- İHA spawn noktası
- Yer kontrol istasyonu için mantıksal alan
- Sabit kanatlı İHA görsel modeli
- Görsel modelden ayrılmış `Rigidbody` fizik kökü
- Gövde, kanat ve iniş takımı için temel collider yapısı
- Automatic Center Of Mass kullanımı
- Pist teması ve yerçekimi Play Mode testi
- Unity Input System tabanlı `Aircraft`, `Camera` ve `UI` Action Map'leri
- Fizik sisteminden bağımsız `AircraftInputReader`
- Klavye ve DualSense gamepad kontrol desteği
- Girdi değerlerini gösteren geliştirme amaçlı debug paneli
- Hatasız ve uyarısız test sahnesi Console kontrolü

---

## MVP Kapsamı

İlk tamamlanabilir sürüm aşağıdakileri içerecektir:

1. Tek bir sabit kanatlı İHA
2. Tek bir havaalanı veya test sahası
3. Klavye, fare ve gamepad kontrolü
4. Temel fizik tabanlı uçuş
5. Kalkış ve iniş
6. Takip kamerası
7. Basit elektro-optik kamera
8. Temel telemetri paneli
9. En az üç waypoint içeren bir görev
10. Görev başarı ekranı
11. Windows çalıştırılabilir build

---

## Kullanılan Teknolojiler

| Teknoloji | Kullanım amacı |
|---|---|
| Unity 6.3 LTS — 6000.3.20f1 | Simülasyon ve oyun motoru |
| C# | Uçuş, görev ve arayüz sistemleri |
| Universal Render Pipeline | Windows için dengeli görsel kalite ve performans |
| Unity Input System | Klavye, fare ve gamepad girdileri |
| Unity Physics | Rigidbody tabanlı uçuş ve çarpışma |
| uGUI ve TextMeshPro | Telemetri ve yer kontrol istasyonu arayüzü |
| Git | Sürüm kontrolü |
| GitHub | Kaynak kod ve portföy sunumu |

### Teknik yaklaşım

- Hedef platform Windows masaüstüdür.
- Renk uzayı `Linear` olarak kullanılmaktadır.
- Uçuş sistemi `Rigidbody` tabanlı yarı gerçekçi bir fizik modeli olarak geliştirilecektir.
- Girdi okuma ile fizik uygulaması birbirinden ayrılacaktır.
- Görsel model ile fizik kök nesnesi birbirinden ayrılmıştır.
- Fizik hesaplamaları `FixedUpdate` içinde yürütülecektir.
- Ana mekanikler hazır bir uçuş sistemi paketine devredilmeyecektir.
- Cinemachine ilk prototip için gerekli görülmemiştir ve kamera fazında yeniden değerlendirilecektir.

Daha ayrıntılı kararlar için [`TECHNICAL_DECISIONS.md`](TECHNICAL_DECISIONS.md) dosyasına bakılabilir.

---

## Mimari Yaklaşım

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

Temel prensipler:

- Kullanıcı girdisi fizik sisteminden bağımsız okunacaktır.
- Inspector ayarları mümkün olduğunca `[SerializeField] private` alanlar üzerinden yönetilecektir.
- Sistemler küçük ve test edilebilir bileşenlere ayrılacaktır.
- Büyük özellikler ayrı Git branchlerinde geliştirilecektir.
- Console hata ve uyarıları geliştirme sürecinde göz ardı edilmeyecektir.
- Kod yapısı teknik görüşmede açıklanabilir olacak şekilde tasarlanacaktır.

---

## Proje Klasör Yapısı

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

Proje tarafından geliştirilen içerikler mümkün olduğunca `Assets/_Project` altında tutulmaktadır. Harici model ve asset paketleri `Assets/ThirdParty` altında izole edilmektedir.

---

## Kontroller

Kontrol şeması Unity Input System üzerinden tanımlanmıştır. Fizik sistemi doğrudan klavye veya gamepad okumaz; girdiler `AircraftInputReader` üzerinden sağlanır.

| İşlem | Klavye / Fare | Gamepad |
|---|---|---|
| Pitch | W / S | Left Stick Y |
| Roll | A / D | Left Stick X |
| Yaw | Q / E | L1 / R1 |
| Throttle artır | Left Shift | R2 |
| Throttle azalt | Left Control | L2 |
| Fren | Space | Button South / Cross |
| Kamera değiştir | C | Button North / Triangle |
| EO kamera zoom | Mouse Wheel | D-Pad Up / Down |
| Pause | Escape | Start / Options |

DualSense kontrolcü Play Mode'da genel `Gamepad` bindingleri üzerinden doğrulanmıştır. Özel joystick/HOTAS desteği MVP sonrasına ertelenmiştir.

---

## Kurulum

Geliştirme için proje **Unity 6.3 LTS — 6000.3.20f1** ile açılmalıdır.

1. Repoyu klonlayın.
2. Unity Hub üzerinden projeyi `6000.3.20f1` Editor sürümüyle açın.
3. Unity'nin paketleri ve assetleri içe aktarmasını bekleyin.
4. `Assets/_Project/Scenes/FlightTest.unity` sahnesini açın.
5. Console'da kritik hata bulunmadığını kontrol edin.
6. Play Mode ile mevcut test ortamını çalıştırın.

```bash
git clone <repository-url>
```

> Repo public olarak yayımlanmadan önce üçüncü taraf assetlerin yeniden dağıtım koşulları ayrıca kontrol edilecektir.

---

## Geliştirme Yol Haritası

- [x] Proje fikrinin ve MVP kapsamının belirlenmesi
- [x] Başlangıç dokümantasyonunun hazırlanması
- [x] Unity projesinin oluşturulması
- [x] Temel klasör ve Git yapısının hazırlanması
- [x] Test havaalanının hazırlanması
- [x] İHA modelinin projeye eklenmesi
- [x] Rigidbody fizik kökü ve temel collider yapısının hazırlanması
- [x] Girdi sisteminin geliştirilmesi
- [ ] Motor ve throttle sisteminin geliştirilmesi
- [ ] Temel uçuş fiziğinin geliştirilmesi
- [ ] Yer hareketi, kalkış ve iniş sistemlerinin geliştirilmesi
- [ ] Kamera sisteminin geliştirilmesi
- [ ] Telemetri arayüzünün geliştirilmesi
- [ ] Waypoint ve görev sisteminin geliştirilmesi
- [ ] EO kamera ve hedefleme sisteminin geliştirilmesi
- [ ] Ses ve görsel iyileştirmeler
- [ ] Optimizasyon
- [ ] Windows build
- [ ] Tanıtım videosu ve portföy sunumu

Ayrıntılı görev listesi için [`TASKS.md`](TASKS.md) dosyasına bakılabilir.

---

## Bilinen Eksikler

Proje henüz uçuş mekaniği geliştirme aşamasına geçmemiştir. Bu nedenle şu sistemler henüz mevcut değildir:

- Motor ve throttle sistemi
- Lift, drag ve kontrol torklarını uygulayan uçuş fiziği
- Kalkış ve iniş sistemi
- Kamera modları
- Telemetri arayüzü
- Yer kontrol istasyonu kullanıcı arayüzü
- Waypoint görevi
- EO kamera hedefleme sistemi
- Windows build

---

## Üçüncü Taraf İçerikler

Projede üçüncü taraf içerikler `Assets/ThirdParty` altında tutulmaktadır.

### Military Base Pack

Test havaalanı ve üs çevresi için **Military Base Pack** kullanılmaktadır.

- Paket, test ortamının çevre modeli olarak kullanılmaktadır.
- Materyalleri URP ile uyumlu hâle getirilmiştir.
- Paket lisansı proje geliştirme sırasında kontrol edilmiştir.
- Ham assetlerin yeniden dağıtım koşulları nedeniyle repo public hâle getirilmeden önce lisans durumu yeniden değerlendirilecektir.

### İHA modeli

İHA görsel modeli Meshy kullanılarak proje için üretilmiştir.

- Model, ücretli Meshy planı kapsamında özel lisans ile oluşturulmuştur.
- Modelin görsel yönü, ölçeği ve fizik hiyerarşisi Unity için ayrıca düzenlenmiştir.
- Uçuş fiziği hazır model davranışına bağlı değildir; proje içinde ayrı olarak geliştirilecektir.

Diğer üçüncü taraf model, ses, doku ve paketler kendi lisanslarına tabidir.

---

## Dokümantasyon

| Dosya | Açıklama |
|---|---|
| [`PROJECT_OVERVIEW.md`](PROJECT_OVERVIEW.md) | Projenin amacı, kapsamı, başarı kriterleri ve güncel durumu |
| [`TECHNICAL_DECISIONS.md`](TECHNICAL_DECISIONS.md) | Alınan teknik kararlar ve gerekçeleri |
| [`TASKS.md`](TASKS.md) | Geliştirme aşamaları ve görev takibi |
| [`README.md`](README.md) | GitHub ve portföy tanıtımı |

---

## Ekran Görüntüleri

Portföy sunumuna uygun ekran görüntüleri proje görsel olarak olgunlaştıkça eklenecektir.

Planlanan yapı:

```text
docs/images/
├── test-environment.png
├── takeoff.png
├── flight.png
├── eo-camera.png
└── landing.png
```

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

## Lisans

Kaynak kod lisansı proje public olarak yayımlanmadan önce belirlenecektir.

Üçüncü taraf içerikler kendi lisanslarına tabidir ve proje reposunun dağıtım biçimi bu lisans koşulları dikkate alınarak belirlenecektir.

---

## Geliştirici

**Mert Kaan**  
Yazılım Mühendisi / Unity Geliştiricisi

Bu proje; Unity, C#, fizik tabanlı sistemler, modüler yazılım mimarisi, teknik dokümantasyon ve sürüm kontrolü yetkinliklerini göstermek amacıyla geliştirilmektedir.
