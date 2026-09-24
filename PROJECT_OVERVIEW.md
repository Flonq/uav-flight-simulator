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
| Oyun motoru | Unity 6000.3.20f1 (Unity 6.3 LTS) |
| Programlama dili | C# |
| Sürüm kontrolü | Git |
| Kod deposu | GitHub — `Flonq/uav-flight-simulator` |
| Geliştirme ortamı | Visual Studio veya JetBrains Rider |
| Girdi sistemi | Unity Input System |
| Fizik sistemi | Unity Rigidbody tabanlı fizik |
| Render Pipeline | Universal Render Pipeline (URP) |
| Kullanıcı arayüzü | uGUI + TextMeshPro |
| Hedef platform | Windows 64-bit |
| Renk uzayı | Linear |

### Ertelenen veya daha sonra doğrulanacak kararlar

- Mevcut takip ve EO kameraları özel bileşenlerle çalışıyor; Cinemachine gereksinimi ileride yeniden değerlendirilecek.
- Joystick/HOTAS desteği MVP sonrasına ertelendi.
- Mini harita mevcut; daha kapsamlı harita çözümü ve yakıt/enerji sistemi henüz kesinleştirilmedi.

Kesinleşen kararların gerekçeleri `TECHNICAL_DECISIONS.md` içinde tutulur.

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
- Gelişmiş otonom pilot
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

Her geliştirme görevi mevcut durumun incelenmesiyle başlar. Değişiklikler kapsamı belirli adımlarla uygulanır, Unity Editor içinde doğrulanır ve sürüm kontrolüne kaydedilir.

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

**Son güncelleme:** 2026-09-24

**Durum:** Faz 13'te yedi waypointli rota, EO hedef gözlemi ve pistte başarılı iniş kullanıcı uçuşunda kabul edildi. Faz 14 ses ve görsel geri bildirim çalışması henüz başlamadı. İniş başarısı ekranda gösteriliyor; görev sonuç ekranı ve yeniden başlatma akışı hâlâ planlıdır.

Aşağıdaki teknik ölçümler 17 Eylül uçuş fiziği temelinin tarihsel doğrulama kayıtlarıdır. Güncel görev ve iniş kabulü için `TASKS.md` ve `TECHNICAL_DECISIONS.md` içindeki TD-037 esas alınır.

Doğrulanmış mevcut durum:

- Unity 6000.3.20f1 / Unity 6.3 LTS projesi oluşturuldu.
- URP, Windows 64-bit, Linear color space ve uGUI + TextMeshPro kararları kesinleşti.
- `FlightTest`, geliştirme ve doğrulama için kullanılan tek üretim sahnesidir.
- Military Base Pack tabanlı test havaalanı yerel olarak çalışıyor ve üçüncü taraf asset dosyaları Git dışında tutuluyor.
- `AircraftRoot > VisualPivot` ayrımı, tek kök Rigidbody ve elle ayarlanmış 10 primitive collider doğrulandı.
- Unity Input System, klavye ve DualSense girdileri ile pasif input debug paneli tamamlandı; panel aktif uçak input okuyucusuna bağlandı.
- Blender 5.2.1 LTS ile geliştirilen özel İHA modeli; ayrı aileronlar, ruddervatorlar, pusher pervane ve iniş takımıyla Unity'ye aktarıldı.
- Modelin metre ölçeği, `-Z` burun yönü, hareketli parça pivotları, materyali, hiyerarşisi ve 44.922 üçgenlik geometri bütçesi doğrulandı.
- `PF_CustomUAVVisual` ve collider ayarlarını koruyan `PF_CustomUAVAircraftPrototype` prefabları oluşturuldu.
- `AircraftControlSurfaceAnimator`, pitch/roll/yaw komutlarını yalnızca görsel yüzey sapmalarına dönüştürecek şekilde eklendi ve Play Mode'da doğrulandı.
- `FlightTest` sahnesindeki özel uçak üç teker üzerinde kararlı duruyor; sıçrama, savrulma veya zeminden geçme gözlenmedi.
- `AircraftEngine`, throttle state/ramp, motor durumu, RPM geçişi ve modelin burun yönü olan root Rigidbody -Z ekseninde itki kuvvetini sabit fizik adımlarında uygular.
- Üç primitive tekerlek sıfır sürtünmeli prototip materyalini korur. `AircraftGroundController`, bu colliderları yer probu olarak kullanır ve Rigidbody üzerinde yönlü tutuş, yuvarlanma direnci, fren, düşük hızlı yaw yönlendirmesi ve pist stabilizasyonu uygular.
- Ground Controller öncesinde -Z pist koşusunda yaklaşık 15° tepe açı değişimi görülüyordu. Güncel kontrollü FlightTest koşusu üç saniyede yaklaşık 12,35 m ilerleme ve 14,14 m/s hız üretirken tepe açı değişimi 0° ve yanal hız yaklaşık 0,00006 m/s ölçüldü.
- `AircraftFollowCamera`, aktif uçağı modelin gerçek arka tarafı olan yerel +Z yönünden 9 m geride ve 3 m yukarıda takip eder. Roll bağımsız chase frame fiziksel `-target.forward` yönünün tam pitch'ini izler; normal uçuşta dünya ufkunu korur, dikey geçişte cache'lenmiş right ekseniyle sürekliliği sağlar ve yüksek hızlı doğrusal hareket için konum feed-forward kullanır. Sahne başlangıcında hedefe sıçramadan yerleşir.
- `AircraftPropellerAnimator`, `AircraftEngine.Rpm` çıktısını tüketerek `Rotor_Pivot` nesnesini yerel Y ekseninde döndürür. Görsel hız ölçeği yüksek RPM'de kare örnekleme kaynaklı alias etkisini azaltır; motor simülasyon değerini değiştirmez.
- `ToggleEngine` komutu klavyede `I`, gamepad'de batı yüz düğmesine bağlıdır. Input Reader yalnızca isteği yayınlar; motor durumu Engine tarafından değiştirilir. Motor kapandığında thrust anında kesilir, RPM ve pervane yaklaşık 1 saniyede rölantiden duruşa iner.
- `AircraftPhysics`, root Rigidbody üzerinde toplam ve gövde eksenli hava hızını, signed angle of attack ve sideslip değerlerini, AoA eğrisinden üretilen lift katsayısını, parasite/induced/stall drag'i, yanal side-force'u, directional-stability yaw momentini ve açısal hız geri beslemeli pitch/roll/yaw kontrolünü sabit fizik adımında uygular. Yanal akış lift ve longitudinal drag hesabından ayrılır; model ileri ekseni root yerel `-Z` olarak korunur.
- Mevcut 1.000 N maksimum itki ve prototip drag modeli doğal olarak yaklaşık 65,71 m/s longitudinal denge üretir. Velocity kesilmeden total airspeed üzerinden `75 m/s` caution, `85 m/s` overspeed ve `80 m/s` recovery eşikleri uygulanır.
- Kontrollü kalkış senaryosunda tam gaz hızlanma sırasında 13 m/s'de burun yukarı komutu verildi. Burun tekeri yaklaşık 15,59 m/s'de; tüm tekerler yaklaşık 17,18 m/s, 3,54 saniye ve 18,94 m pist mesafesinde yerden ayrıldı. Roll sapması ölçülmedi ve pist orta hattı sapması 0,002 m altında kaldı.
- Input Debug paneli toplam hava hızı, ileri ve yanal hava hızı, sideslip açısı, dikey hız, angle of attack, uçuş durumu, hız zarfı durumu/overspeed eşiği ve üç teker temas durumunu gösterir.
- Stall ve post-stall durumları, ileri hız izdüşümü azaldığında artık kontrol otoritesi ve açısal hız sönümlemesi FlightTest'te doğrulandı.
- Prototip hız zarfı tamamlandı; eşiklerin gerçek Vne/işletme limitleriyle kalibrasyonu, gerçek kütle/ağırlık merkezi/inertia, propeller verimi, rüzgâr/irtifa yoğunluğu ve gerçek araç kalkış kalibrasyonu henüz sonlandırılmadı.

17 Eylül'e kadar tamamlanan kontrollü akış:

1. Tamamlandı (2026-09-15): `AircraftInputReader` ve `AircraftControlSurfaceAnimator` uçak prefabına taşındı; debug panel sahnede aktif instance'a bağlı kaldı (TD-022).
2. Tamamlandı (2026-09-15): Throttle state/ramp sahipliği `AircraftEngine` bileşenine taşındı; Input Reader yalnızca komut sağlar.
3. Tamamlandı (2026-09-15): Motor RPM, prototip thrust ve propulsion kuvveti fixed-timestep yolunda uygulandı; kısa pist testi doğrulandı.
4. Tamamlandı (2026-09-17): Takip kamerası tam pitch izleyen roll bağımsız chase frame, dikey geçiş sürekliliği, heading/right cache'i ve yüksek hızlı konum feed-forward ile stabilize edildi; Play Mode ölçümleri doğrulandı.
5. Tamamlandı (2026-09-16): `Rotor_Pivot` görsel dönüşü motor RPM verisine bağlandı.
6. Tamamlandı (2026-09-16): Motor açma/kapatma komutu ve kademeli pervane duruşu uygulandı.
7. Tamamlandı (2026-09-16): Geçici model inceleme sahnesi kaldırıldı; doğrulama akışı `FlightTest` ve üretim prefabında birleştirildi.
8. Tamamlandı (2026-09-16): Temel aerodinamik lift, drag ve hıza bağlı kontrol torkları ayrı `AircraftPhysics` bileşeninde uygulandı.
9. Tamamlandı (2026-09-16): Yönlü yer tutuşu, yuvarlanma direnci, düşük hızlı yönlendirme, pist stabilizasyonu ve fren ayrı `AircraftGroundController` bileşeninde uygulandı.
10. Tamamlandı (2026-09-16): Aerodinamik ve yer hareketi birlikte çalışırken 13 m/s rotasyon komutlu kontrollü kalkış senaryosu ve yaklaşık 17,2 m/s prototip yerden kesilme referansı doğrulandı.
11. Tamamlandı (2026-09-17): Toplam hava hızı ve signed angle of attack tabanlı lift, stall sonrası drag, artık kontrol otoritesi ve rate feedback uygulandı; nötr pitch, kontrollü kalkış ve sönümlenen pitch senaryoları doğrulandı.
12. Tamamlandı (2026-09-17): Gövde eksenli yanal hava hızı/sideslip telemetrisi, side-force, directional stability, yanal akıştan ayrılmış lift/longitudinal drag ve geri-akış kontrol sınırı uygulandı; 20/20 EditMode testi ve kontrollü FlightTest ölçümleri doğrulandı.
13. Tamamlandı (2026-09-17): Doğal thrust/drag dengesi yaklaşık 65,71 m/s olarak doğrulandı; total airspeed tabanlı 75/85/80 m/s caution/overspeed/recovery zarfı ve hysteresis debug telemetrisi uygulandı.
14. Sonraki fazlarda prototip hız zarfı ve aerodinamik katsayılar gerçek araç verisi sağlandığında yeniden kalibre edilecek.

---

## 14. İletişim ve Proje Sahibi

**Geliştirici:** Mert Kaan  
**Rol:** Yazılım Mühendisi / Unity Geliştiricisi  
**Proje türü:** Bireysel portföy projesi
