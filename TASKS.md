# TASKS

Bu dosya projenin geliştirme planını ve ilerleme durumunu takip etmek için kullanılacaktır.

## İşaretler

- `[ ]` Başlanmadı
- `[x]` Tamamlandı
- `[~]` Devam ediyor
- `[!]` Engellendi veya sorun var

> GitHub standart Markdown görev listeleri `[ ]` ve `[x]` biçimini destekler. Devam eden ve engellenen görevlerde açıklama satırı kullanılabilir.

---

# FAZ 0 — Planlama ve Dokümantasyon

- [x] Projenin genel amacı belirlendi
- [x] MVP kapsamı taslağı oluşturuldu
- [x] `PROJECT_OVERVIEW.md` oluşturuldu
- [x] `TECHNICAL_DECISIONS.md` oluşturuldu
- [x] `TASKS.md` oluşturuldu
- [x] `README.md` oluşturuldu
- [x] Unity sürümünü kesinleştir — `6000.3.20f1 / Unity 6.3 LTS`
- [x] Render Pipeline seçimini kesinleştir — URP
- [x] UI teknolojisini kesinleştir — uGUI + TextMeshPro
- [x] İHA modelini kesinleştir — Blender'da geliştirilen özel UAV modeli
- [ ] Proje takvimini oluştur

### Çıkış kriteri

Projenin teknik temeli ve ilk sürüm kapsamı açıkça tanımlanmış olmalıdır.

---

# FAZ 1 — Unity Proje Kurulumu

- [x] Yeni Unity projesini oluştur
- [x] Proje adını belirle — `UAVFlightSimulator`
- [x] Windows 64-bit hedef platformunu ayarla
- [x] Linear renk uzayını ayarla
- [x] URP kurulumunu doğrula
- [x] Input System paketini kur
- [x] TextMeshPro temel kaynaklarını ekle
- [x] Cinemachine'i şimdilik kullanmama kararını kaydet
- [x] `_Project` ana klasörünü oluştur
- [x] Önerilen alt klasör yapısını oluştur
- [x] İlk test sahnesini oluştur
- [x] Sahneyi `FlightTest` adıyla kaydet
- [x] `AssetReview` sahnesini oluştur
- [x] Asset Serialization ayarını Force Text yap
- [x] Proje/Git güvenlik yedeklerini al
- [x] Git deposunu başlat
- [x] Unity için `.gitignore` ekle
- [x] İlk commit'i oluştur
- [x] GitHub reposunu oluştur — `Flonq/uav-flight-simulator`
- [x] Yerel repoyu GitHub'a push et

### Çıkış kriteri

Proje hatasız açılmalı, boş test sahnesi çalışmalı ve GitHub üzerinde ilk sürüm bulunmalıdır.

---

# FAZ 2 — Test Ortamı ve Havaalanı

- [x] Military Base Pack tabanlı test ortamını yerel olarak kur
- [x] Pist ve zemin alanını hazırla
- [x] Pist/zemin temasını collider ile doğrula
- [x] İHA spawn noktasını oluştur
- [x] Hangar ve üs çevresini test ortamında hazırla
- [x] Mantıksal `GroundControlStationArea` işaretini oluştur
- [x] Directional Light ayarla
- [x] Skybox ayarla
- [x] Sis ayarını değerlendir — kapalı
- [x] Military Base Pack materyallerini `URP/Lit` ile uyumlu hâle getir
- [x] 53 Spot Light için gölgeleri kapat
- [x] Çevre ölçeğini metre birimine göre kontrol et
- [x] Git geçmişinden yeniden dağıtılamayan üçüncü taraf assetleri temizle
- [x] Military Base Pack klasörünün yerel kalıp Git tarafından yok sayıldığını doğrula
- [x] Git temizliği sonrasında Unity smoke testini tamamla
- [ ] Havaalanı sahnesinde 60 FPS testi yap

### Çıkış kriteri

İHA fizik testleri için ölçeği doğru ve performanslı bir test sahası hazırlanmış olmalıdır.

---

# FAZ 3 — İHA Modeli ve Fizik Kökü

## 3A — Mevcut Unity prototipi

- [x] Geçici/önceki UAV görselini Unity'ye aktar
- [x] `AircraftRoot > VisualPivot` ayrımını oluştur
- [x] Görsel modeli fizik kökünün altına yerleştir
- [x] Rigidbody ekle
- [x] Prototip kütle değerini `100` olarak belirle
- [x] Automatic Center of Mass ile başlangıç testi yap
- [x] Ana gövde capsule collider prototipini oluştur
- [x] Kanat box collider prototipini oluştur
- [x] Tekerlek sphere collider prototiplerini oluştur
- [x] Yer çekimi ve pist temasını doğrula
- [x] `PF_AircraftVisualPrototype` prefabını oluştur
- [x] `PF_AircraftPrototype` prefabını oluştur
- [x] Prototipi `FlightTest` sahnesine ekle

## 3B — Özel Blender İHA modeli

- [x] Özel UAV modelini Blender 5.2.1 LTS ile sıfırdan oluştur
- [x] Fuselage, ana kanat, tail boom, V-tail, pusher pervane ve iniş takımlarını tamamla
- [x] Sağ/sol aileronları ayrı hareketli mesh olarak tamamla
- [x] Sağ/sol ruddervatorları ayrı hareketli mesh olarak tamamla
- [x] Kontrol yüzeylerindeki son görsel/geometrik sorunları düzelt
- [x] Model sahipliğini doğrula — proje için geliştirici tarafından oluşturulan özel model
- [x] Unity export kopyasını ve temiz export hiyerarşisini hazırla
- [x] Blender referans/helper/cutter/guide nesnelerinin export dışında kaldığını doğrula
- [x] Modeli Unity projesindeki `Assets/_Project/Art/Aircraft/CustomUAV/` alanına aktar
- [x] Import ölçeğini metre birimine göre doğrula
- [x] Modelin Unity yerel `+Z` burun yönünü doğrula
- [x] Aileron, ruddervator ve propeller pivot/yerel eksenlerini Unity'de doğrula
- [x] Normals, tangents, materyal slotları ve triangle sayısını doğrula
- [x] Özel görseli mevcut `VisualPivot` altında test et
- [x] Mevcut geçici görseli yalnızca yeni model doğrulandıktan sonra devre dışı bırak
- [~] Collider ve Center of Mass değerlerini yeni model boyutlarına göre yeniden değerlendir — 10 collider ve otomatik COM ile kararlı; gerçek kütle/CG verisi bekleniyor
- [x] Mevcut uçak prefabını güvenli biçimde güncelle — Input Reader ve Animator prefab kökünde; elle ayarlanmış fizik yapısı korundu (2026-09-15)
- [ ] `AssetReview` sahnesinde final model Play Mode smoke testi yap
- [x] `FlightTest` sahnesinde final model Play Mode smoke testi yap
- [x] Aileron ve ruddervatorları input komutlarıyla görsel olarak hareket ettir

### Çıkış kriteri

İHA modeli sahnede doğru ölçekte, doğru yönde ve fizik bileşenleriyle hazır bulunmalıdır.

---

# FAZ 4 — Girdi Sistemi

- [x] `Aircraft.inputactions` dosyasını oluştur
- [x] Pitch action oluştur
- [x] Roll action oluştur
- [x] Yaw action oluştur
- [x] Throttle action oluştur
- [x] Brake action oluştur
- [x] Kamera değiştirme action oluştur
- [x] EO kamera zoom action oluştur
- [x] Pause action oluştur
- [x] C# sınıf üretimini etkinleştir
- [x] `AircraftInputReader` scriptini oluştur
- [x] Girdi değerlerini pasif debug panelinde göster
- [x] Debug panelini sahnedeki aktif `AircraftInputReader` bileşenine bağla
- [x] Klavye ve fare girdilerini test et
- [x] DualSense'i genel `<Gamepad>` bindingleriyle test et
- [x] Joystick/HOTAS desteğini MVP sonrasına ertele
- [x] Throttle sahipliğini `AircraftInputReader` → `AircraftEngine` yönünde refactor et (2026-09-15)

### Çıkış kriteri

Bütün kullanıcı komutları fizik sisteminden bağımsız şekilde okunabilmelidir.

---

# FAZ 5 — Motor ve Throttle Sistemi

- [x] `AircraftEngine` scriptini oluştur — throttle state/ramp temeli
- [x] Minimum throttle tanımla — 0
- [x] Maksimum throttle tanımla — 1
- [x] Throttle artış ve azalış hızını belirle — 0,5/s; FixedUpdate
- [ ] Motor thrust değerini hesapla
- [ ] İleri yön kuvvetini uygula
- [ ] Motor açık ve kapalı durumu ekle
- [ ] Motor devri değerini üret
- [ ] Motor sesi için temel parametre üret
- [ ] Inspector ayarlarını grupla
- [ ] Pist üzerinde hızlanmayı test et
- [ ] Maksimum yer hızını kontrol et

### Çıkış kriteri

İHA throttle komutuyla pist üzerinde kontrollü şekilde hızlanabilmelidir.

---

# FAZ 6 — Temel Uçuş Fiziği

- [ ] `AircraftPhysics` scriptini oluştur
- [ ] Hava hızını hesapla
- [ ] Lift kuvvetini hesapla
- [ ] Drag kuvvetini hesapla
- [ ] Pitch torkunu uygula
- [ ] Roll torkunu uygula
- [ ] Yaw torkunu uygula
- [ ] Hıza bağlı kontrol etkinliği ekle
- [ ] Düşük hız kontrol sınırı ekle
- [ ] Basitleştirilmiş stall davranışı ekle
- [ ] Maksimum güvenli hız davranışı ekle
- [ ] Fizik ayarlarını Inspector üzerinden düzenlenebilir yap
- [ ] Debug kuvvet çizimleri ekle
- [ ] Sabit FPS bağımsızlığını test et
- [ ] 30, 60 ve 120 FPS testleri yap

### Çıkış kriteri

İHA havalanabilmeli ve pitch, roll, yaw eksenlerinde kararlı biçimde kontrol edilebilmelidir.

---

# FAZ 7 — Yer Hareketi ve Kalkış

- [ ] `AircraftGroundController` scriptini oluştur
- [ ] Yerde olma kontrolü ekle
- [ ] Tekerlek sürtünmesini ayarla
- [ ] Yaw ile pist yönlendirmesi ekle
- [ ] Fren sistemi ekle
- [ ] Pistte yana kaymayı azalt
- [ ] Kalkış hızını belirle
- [ ] Kalkış test senaryosu oluştur
- [ ] Pist dışına çıkma davranışını kontrol et
- [ ] Tekerleklerin görsel animasyonunu değerlendir
- [ ] İniş takımı sistemi sonraki faz için not et

### Çıkış kriteri

İHA pist merkez çizgisinde hızlanabilmeli ve kontrollü şekilde havalanabilmelidir.

---

# FAZ 8 — Kamera Sistemi

- [ ] `CameraModeController` oluştur
- [ ] Takip kamerası ekle
- [ ] Takip kamerasında yumuşak hareket ekle
- [ ] Gövde kamerası ekle
- [ ] Serbest kamera ekle
- [ ] EO kamera ekle
- [ ] Kamera modları arasında geçiş ekle
- [ ] EO kamera zoom sistemi ekle
- [ ] EO kamera dönüş limitleri ekle
- [ ] Kamera titreşimini değerlendir
- [ ] Kamera geçişlerinde görüntü sıçramasını önle
- [ ] Kamera modunu UI üzerinde göster

### Çıkış kriteri

Kullanıcı uçuş ve görev sırasında farklı kamera modlarını sorunsuz kullanabilmelidir.

---

# FAZ 9 — Telemetri Sistemi

- [ ] `AircraftTelemetry` scriptini oluştur
- [ ] Ground speed hesapla
- [ ] Air speed hesapla
- [ ] İrtifa hesapla
- [ ] Dikey hız hesapla
- [ ] Heading hesapla
- [ ] Pitch açısını hesapla
- [ ] Roll açısını hesapla
- [ ] Yaw açısını hesapla
- [ ] Throttle yüzdesini hesapla
- [ ] Motor durumunu hesapla
- [ ] Waypoint mesafesini hesapla
- [ ] Telemetri verilerini event veya arayüz üzerinden yayınla
- [ ] Telemetri değerlerinin doğruluğunu test et

### Çıkış kriteri

Uçuş sisteminden gerekli bütün temel veriler UI tarafından okunabilir durumda olmalıdır.

---

# FAZ 10 — Yer Kontrol İstasyonu Arayüzü

- [ ] Ana HUD taslağını hazırla
- [ ] Hız göstergesini ekle
- [ ] İrtifa göstergesini ekle
- [ ] Dikey hız göstergesini ekle
- [ ] Heading göstergesini ekle
- [ ] Throttle göstergesini ekle
- [ ] Uçuş modu göstergesini ekle
- [ ] Kamera modu göstergesini ekle
- [ ] Uyarı mesaj alanı ekle
- [ ] Görev hedefi alanı ekle
- [ ] EO kamera görüntü paneli ekle
- [ ] Mini harita alanını değerlendir
- [ ] Arayüzü 16:9 çözünürlüklerde test et
- [ ] 1920×1080 çözünürlük testi yap
- [ ] 1366×768 çözünürlük testi yap

### Çıkış kriteri

Kullanıcı temel uçuş ve görev bilgilerini tek ekrandan takip edebilmelidir.

---

# FAZ 11 — Waypoint ve Görev Sistemi

- [ ] `MissionManager` oluştur
- [ ] `Waypoint` bileşeni oluştur
- [ ] Waypoint sıralaması ekle
- [ ] Waypoint algılama yarıçapı ekle
- [ ] Aktif waypoint görseli ekle
- [ ] Görev başlangıç sistemi ekle
- [ ] Görev açıklama paneli ekle
- [ ] Hedef bölgesi oluştur
- [ ] EO kamera ile hedef gözlemi koşulu ekle
- [ ] Görev başarı koşulu ekle
- [ ] Görev başarısızlık koşulu ekle
- [ ] Görev süresi ekle
- [ ] Üsse dönüş waypointleri ekle
- [ ] İniş sonrası görevi tamamlama koşulu ekle
- [ ] Görev sonuç ekranı ekle

### Çıkış kriteri

Kullanıcı kalkıştan inişe kadar tamamlanabilir tek bir görev oynayabilmelidir.

---

# FAZ 12 — Hedefleme ve EO Kamera

- [ ] Hedef nesnesi oluştur
- [ ] Hedef tespit alanı ekle
- [ ] EO kamera merkez noktasını hesapla
- [ ] Raycast ile hedef tespiti ekle
- [ ] Hedef kilidi sistemi ekle
- [ ] Hedef işaretleyici ekle
- [ ] Zoom seviyelerini ayarla
- [ ] Kamera dönüş hızını ayarla
- [ ] Görüş alanı sınırlarını ayarla
- [ ] Hedef kaybolma durumunu yönet
- [ ] Görev sistemiyle bağlantı kur
- [ ] Yanlış hedef davranışını test et

### Çıkış kriteri

Kullanıcı EO kamera ile görev hedefini bulabilmeli ve doğrulayabilmelidir.

---

# FAZ 13 — İniş Sistemi

- [ ] Yaklaşma waypointleri oluştur
- [ ] Pist yönlendirme göstergesi ekle
- [ ] Yüksek dikey hız uyarısı ekle
- [ ] İniş takımı durumunu değerlendir
- [ ] Yere temas algılama sistemi oluştur
- [ ] Sert iniş algılama sistemi ekle
- [ ] Pist üzerinde frenleme davranışını ayarla
- [ ] Pist dışı iniş başarısızlık koşulu ekle
- [ ] Başarılı iniş koşulu ekle
- [ ] Farklı yaklaşma hızlarında test yap

### Çıkış kriteri

İHA kontrollü şekilde piste indirilebilmeli ve durdurulabilmelidir.

---

# FAZ 14 — Ses ve Görsel Geri Bildirim

- [ ] Motor sesi ekle
- [ ] Motor sesini throttle değerine bağla
- [ ] Rüzgâr sesi ekle
- [ ] Pist temas sesi ekle
- [ ] Uyarı sesi ekle
- [ ] Kamera geçiş sesi değerlendir
- [ ] UI buton sesleri ekle
- [ ] Hafif kamera titreşimi değerlendir
- [ ] Post-processing ayarlarını yap
- [ ] Sis ve atmosfer görünümünü iyileştir
- [ ] Gün ışığı ayarını iyileştir
- [ ] Gereksiz görsel efektleri kaldır

### Çıkış kriteri

Simülasyon temel seviyede tutarlı ses ve görsel geri bildirim sunmalıdır.

---

# FAZ 15 — Ayarlar ve Kullanılabilirlik

- [ ] Ana menü oluştur
- [ ] Görev başlat butonu ekle
- [ ] Kontroller ekranı ekle
- [ ] Ses ayarları ekle
- [ ] Grafik ayarları ekle
- [ ] Fare hassasiyeti ayarı ekle
- [ ] Kamera hassasiyeti ayarı ekle
- [ ] Pause menüsü ekle
- [ ] Görevi yeniden başlat seçeneği ekle
- [ ] Ana menüye dön seçeneği ekle
- [ ] Uygulamadan çık seçeneği ekle

### Çıkış kriteri

Kullanıcı simülasyonu menüler üzerinden başlatabilmeli, durdurabilmeli ve temel ayarları değiştirebilmelidir.

---

# FAZ 16 — Hata Ayıklama ve Optimizasyon

- [ ] Console üzerindeki bütün hata mesajlarını temizle
- [ ] Console üzerindeki önemli warning mesajlarını temizle
- [ ] Unity Profiler ile CPU kullanımını incele
- [ ] Unity Profiler ile GPU kullanımını incele
- [ ] GC allocation noktalarını incele
- [ ] Physics ayarlarını optimize et
- [ ] LOD sistemlerini kontrol et
- [ ] Gölge ayarlarını optimize et
- [ ] Gereksiz colliderları kaldır
- [ ] Gereksiz Update metodlarını kaldır
- [ ] Build boyutunu kontrol et
- [ ] Uzun süreli uçuş testi yap
- [ ] Sahne yeniden başlatma testi yap
- [ ] Düşük FPS testi yap

### Çıkış kriteri

Simülasyon hedef bilgisayarda kararlı çalışmalı ve kritik performans sorunu içermemelidir.

---

# FAZ 17 — Build ve Dağıtım

- [ ] Windows build ayarlarını yap
- [ ] Ürün adını ayarla
- [ ] Uygulama ikonunu ekle
- [ ] Sürüm numarasını belirle
- [ ] Development Build kapalı test build al
- [ ] Temiz bilgisayarda build testi yap
- [ ] Eksik DLL veya dosya kontrolü yap
- [ ] Grafik ayarlarını test et
- [ ] Kontrol şemasını test et
- [ ] Build klasörünü sıkıştır
- [ ] GitHub Release oluşturmayı değerlendir
- [ ] İndirme ve çalıştırma talimatlarını yaz

### Çıkış kriteri

Başka bir Windows bilgisayarda kurulumsuz veya açık talimatlarla çalıştırılabilir bir sürüm hazırlanmalıdır.

---

# FAZ 18 — Portföy ve Başvuru Hazırlığı

- [ ] Profesyonel ekran görüntüleri al
- [ ] Kalkış videosu kaydet
- [ ] EO kamera videosu kaydet
- [ ] İniş videosu kaydet
- [ ] Kısa proje tanıtım videosu hazırla
- [ ] README görsellerini ekle
- [ ] Mimari diyagram oluştur
- [ ] Kullanılan teknolojileri güncelle
- [ ] Bilinen sorunları yaz
- [ ] Gelecek geliştirmeleri yaz
- [ ] GitHub reposunu temizle
- [ ] Commit geçmişini kontrol et
- [ ] Lisans dosyalarını kontrol et
- [ ] CV için proje açıklaması yaz
- [ ] LinkedIn için proje paylaşımı hazırla
- [ ] Teknik görüşmede anlatılacak noktaları hazırla

### Çıkış kriteri

Proje GitHub, CV ve teknik görüşmede profesyonel biçimde sunulabilir olmalıdır.

---

# HATA VE TEKNİK BORÇ LİSTESİ

Yeni bir hata bulunduğunda aşağıdaki biçimde eklenmelidir:

```text
- [ ] Kısa hata başlığı
  - Beklenen davranış:
  - Gerçekleşen davranış:
  - Tekrarlama adımları:
  - İlgili dosyalar:
  - Öncelik:
```

Mevcut doğrulanması gereken teknik borçlar:

- [x] Kök dokümantasyon ile repository durumunu karşılaştır
- [~] `FlightTest` içindeki eski/inactive uçak instance'larını koru ve final sistem kabulünden sonra temizle
- [x] `AircraftInputReader` içindeki throttle state/ramp sorumluluğunu `AircraftEngine` bileşenine taşı
- [ ] Military Base Pack'in `.gitignore` kuralını ve temiz Git geçmişini koru
- [x] Özel UAV modelinin Unity import/eksen/pivot/materyal testini tamamla
- [ ] Gerçek kütle, ağırlık merkezi ve inertia değerlerini fiziksel verilerle kalibre et
- [x] `AircraftControlSurfaceAnimator` ve `AircraftInputReader` sahipliğini uçak prefabında kesinleştir (TD-022)

---

# SONRAKİ ÜÇ GÖREV

Bu bölüm her çalışma oturumunun sonunda güncellenmelidir.

1. Motor RPM, thrust ve fixed-timestep propulsion uygulamasını geliştir.
2. `Rotor_Pivot` görselini motor RPM verisine bağla.
3. `AssetReview` sahnesinde final model Play Mode smoke testini tamamla.
