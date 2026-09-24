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
- [x] Geçici model inceleme sahnesini entegrasyon tamamlandıktan sonra kaldır
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
- [x] Modelin Unity yerel `-Z` burun yönünü doğrula — NoseGear önde, pusher rotor arkada
- [x] Aileron, ruddervator ve propeller pivot/yerel eksenlerini Unity'de doğrula
- [x] Normals, tangents, materyal slotları ve triangle sayısını doğrula
- [x] Özel görseli mevcut `VisualPivot` altında test et
- [x] Mevcut geçici görseli yalnızca yeni model doğrulandıktan sonra devre dışı bırak
- [~] Collider ve Center of Mass değerlerini yeni model boyutlarına göre yeniden değerlendir — 10 collider ve otomatik COM ile kararlı; gerçek kütle/CG verisi bekleniyor
- [x] Mevcut uçak prefabını güvenli biçimde güncelle — Input Reader ve Animator prefab kökünde; elle ayarlanmış fizik yapısı korundu (2026-09-15)
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
- [x] Motor açma/kapatma action oluştur — klavye `I`, gamepad batı yüz düğmesi
- [x] Kamera değiştirme action oluştur
- [x] EO kamera zoom action oluştur
- [x] Pause action oluştur
- [x] C# sınıf üretimini etkinleştir
- [x] `AircraftInputReader` scriptini oluştur
- [x] Girdi değerlerini pasif debug panelinde göster
- [x] Debug panelinde klavye/fare tuş atamalarını Input Action tanımlarından göster
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
- [x] Motor thrust değerini hesapla — rölanti üzerindeki normalize RPM'nin karesi; prototip
- [x] İleri yön kuvvetini uygula — modelin burun yönü olan root Rigidbody -Z; FixedUpdate / ForceMode.Force
- [x] Motor açık ve kapalı durumu ekle — API, `I`/gamepad toggle komutu ve Play Mode bileşen menüsü
- [x] Motor devri değerini üret — rölanti/maksimum RPM ve geçiş hızı
- [x] Motor kapanışında ayrı RPM düşüş hızı uygula — 1.200 RPM/s; rölantiden yaklaşık 1 saniyede duruş
- [x] `Rotor_Pivot` görsel dönüşünü `AircraftEngine.Rpm` verisine bağla
- [x] Motor sesi için temel parametre üret — Rpm ve NormalizedRpm; ses bağlantısı ayrı fazda
- [x] Inspector ayarlarını grupla
- [x] Pist üzerinde hızlanmayı test et — kontrollü üç saniyelik gaz rampası (2026-09-15)
- [ ] Maksimum yer hızını kontrol et

### Çıkış kriteri

İHA throttle komutuyla pist üzerinde kontrollü şekilde hızlanabilmelidir.

---

# FAZ 6 — Temel Uçuş Fiziği

- [x] `AircraftPhysics` scriptini oluştur
- [x] Hava hızını hesapla
- [x] Lift kuvvetini hesapla
- [x] Drag kuvvetini hesapla
- [x] Pitch torkunu uygula
- [x] Roll torkunu uygula
- [x] Yaw torkunu uygula
- [x] Hıza bağlı kontrol etkinliği ekle
- [x] Düşük hız kontrol sınırı ekle
- [x] Signed angle of attack ve toplam hava hızına dayalı basitleştirilmiş stall davranışı ekle
- [x] Stall sonrası drag artışı ve ileri hız izdüşümünden bağımsız artık kontrol otoritesi ekle
- [x] Açısal hız hedefi, rate feedback, eksen damping'i ve açısal ivme sınırlarını ekle
- [x] Gövde eksenli signed lateral airspeed ve sideslip açısını hesapla (2026-09-17)
- [x] Sideslip'e karşı yanal side-force ve yönelme kararlılığı uygula (2026-09-17)
- [x] Lift/longitudinal drag akışını yanal hızdan ayır ve geri akış kontrol otoritesini sınırla (2026-09-17)
- [x] Sideslip decay ve 10/20/40 ms sabit adım regresyonlarını ekle (2026-09-17)
- [x] Total airspeed tabanlı prototip hız zarfı, overspeed hysteresis ve debug telemetrisi ekle (2026-09-17)
- [x] Tam gaz doğal thrust/drag dengesini ve 10/20/40 ms hız tutarlılığını doğrula — yaklaşık 65,71 m/s (2026-09-17)
- [x] Fizik ayarlarını Inspector üzerinden düzenlenebilir yap
- [ ] Debug kuvvet çizimleri ekle
- [x] Sabit fizik adımı bağımsızlığını kontrollü 10/20/40 ms adımlarda test et
- [ ] 30, 60 ve 120 FPS testleri yap

### Çıkış kriteri

İHA havalanabilmeli, pitch/roll/yaw komutlarıyla kontrol edilebilmeli ve yüksek yanal hız veya sideslip nötr girdilerde kararlı biçimde toparlanabilmelidir.

---

# FAZ 7 — Yer Hareketi ve Kalkış

- [x] `AircraftGroundController` scriptini oluştur
- [x] Yerde olma kontrolü ekle — üç mevcut tekerlek SphereCollider probu
- [x] Tekerlek sürtünmesini ayarla — kuvvet tabanlı yönlü tutuş ve yuvarlanma direnci prototipi
- [x] Yaw ile pist yönlendirmesi ekle
- [x] Fren sistemi ekle
- [x] Pistte yana kaymayı azalt
- [ ] Kalkış hızını belirle
- [ ] Kalkış test senaryosu oluştur
- [ ] Pist dışına çıkma davranışını kontrol et
- [ ] Tekerleklerin görsel animasyonunu değerlendir
- [x] İniş takımı sistemi sonraki faz için not et — mevcut primitive tekerlekler süspansiyon veya dönen tekerlek fiziği sağlamaz

### Çıkış kriteri

İHA pist merkez çizgisinde hızlanabilmeli ve kontrollü şekilde havalanabilmelidir.

---

# FAZ 8 — Kamera Sistemi

- [x] `CameraModeController` oluştur
- [x] Takip kamerası ekle
- [x] Takip kamerasında yumuşak hareket ekle
- [ ] Gövde kamerası ekle
- [ ] Serbest kamera ekle
- [x] EO kamera ekle
- [x] Kamera modları arasında geçiş ekle
- [x] EO kamera zoom sistemi ekle
- [x] EO kamera dönüş limitleri ekle
- [ ] Kamera titreşimini değerlendir
- [x] Kamera geçişlerinde görüntü sıçramasını önle
- [x] Kamera modunu UI üzerinde göster

### Çıkış kriteri

Kullanıcı uçuş ve görev sırasında farklı kamera modlarını sorunsuz kullanabilmelidir.

---

# FAZ 9 — Telemetri Sistemi

- [x] `AircraftTelemetry` scriptini oluştur
- [x] Ground speed hesapla
- [x] Air speed hesapla
- [x] İrtifa hesapla
- [x] Dikey hız hesapla
- [x] Heading hesapla
- [x] Pitch açısını hesapla
- [x] Roll açısını hesapla
- [x] Yaw açısını hesapla
- [x] Throttle yüzdesini hesapla
- [x] Motor durumunu hesapla
- [x] Waypoint mesafesini hesapla — MissionSnapshot ve production HUD tarafından tüketiliyor
- [x] Telemetri verilerini event veya arayüz üzerinden yayınla
- [x] Telemetri değerlerinin doğruluğunu test et

### Çıkış kriteri

Uçuş sisteminden gerekli bütün temel veriler UI tarafından okunabilir durumda olmalıdır.

---

# FAZ 10 — Yer Kontrol İstasyonu Arayüzü

- [x] Ana HUD taslağını hazırla
- [x] Hız göstergesini ekle
- [x] İrtifa göstergesini ekle
- [x] Dikey hız göstergesini ekle
- [x] Heading göstergesini ekle
- [x] Throttle göstergesini ekle
- [x] Uçuş modu göstergesini ekle
- [x] Kamera modu göstergesini ekle
- [x] Uyarı mesaj alanı ekle
- [x] Görev hedefi alanı ekle
- [x] EO kamera görüntü paneli ekle
- [x] Mini harita alanını değerlendir
- [x] Arayüzü 16:9 çözünürlüklerde test et
- [x] 1920×1080 çözünürlük testi yap
- [x] 1366×768 çözünürlük testi yap

### Çıkış kriteri

Kullanıcı temel uçuş ve görev bilgilerini tek ekrandan takip edebilmelidir.

---

# FAZ 11 — Waypoint ve Görev Sistemi

- [x] `MissionManager` oluştur
- [x] `Waypoint` bileşeni oluştur
- [x] Waypoint sıralaması ekle
- [x] Waypoint algılama yarıçapı ekle
- [x] Aktif waypoint görseli ekle
- [x] Görev başlangıç sistemi ekle
- [x] Görev açıklama paneli ekle
- [x] Hedef bölgesi oluştur
- [x] EO kamera ile hedef gözlemi koşulu ekle
- [ ] Görev başarı koşulu ekle
- [ ] Görev başarısızlık koşulu ekle
- [ ] Görev süresi ekle
- [x] Üsse dönüş waypointleri ekle
- [ ] İniş sonrası görevi tamamlama koşulu ekle
- [ ] Görev sonuç ekranı ekle

### Çıkış kriteri

Kullanıcı kalkıştan inişe kadar tamamlanabilir tek bir görev oynayabilmelidir.

---

# FAZ 12 — Hedefleme ve EO Kamera

- [x] Hedef nesnesi oluştur
- [x] Hedef tespit alanı ekle
- [x] EO kamera merkez noktasını hesapla
- [x] Raycast ile hedef tespiti ekle
- [x] Hedef kilidi sistemi ekle
- [x] Hedef işaretleyici ekle
- [x] Zoom seviyelerini ayarla
- [x] Kamera dönüş hızını ayarla
- [x] Görüş alanı sınırlarını ayarla
- [x] Hedef kaybolma durumunu yönet
- [x] Görev sistemiyle bağlantı kur
- [x] Yanlış hedef davranışını test et

### Çıkış kriteri

Kullanıcı EO kamera ile görev hedefini bulabilmeli ve doğrulayabilmelidir.

---

# FAZ 13 — İniş Sistemi

- [x] Yaklaşma waypointleri oluştur
- [x] Pist yönlendirme göstergesi ekle
- [x] Yüksek dikey hız uyarısı ekle
- [x] İniş takımı durumunu değerlendir
- [x] Yere temas algılama sistemi oluştur
- [x] Sert iniş algılama sistemi ekle
- [x] Pist üzerinde frenleme davranışını ayarla — 5 m/s² prototip değeri kontrollü pist ölçümünde doğrulandı; kullanıcı fren hissini ve yön kararlılığını kabul etti
- [x] Pist dışı iniş başarısızlık koşulu ekle
- [x] Başarılı iniş koşulu ekle
- [x] Farklı yaklaşma hızlarında test yap

### Çıkış kriteri

İHA kontrollü şekilde piste indirilebilmeli ve durdurulabilmelidir.

Kullanıcı 24 Eylül 2026'da rota `7/7`, EO hedef gözlemi `Observed` ve pistte fren sonrası `LANDING SUCCESSFUL` durumunu manuel uçuşta kabul etti.

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
- [ ] Görevi yeniden başlat seçeneği ekle — rota, kalıcı EO gözlemi, iniş monitörü ve uçak başlangıç durumu birlikte sıfırlanmalı
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
- [x] Üç tekerlekteki sıfır temas sürtünmesini yönlü yer tutuşu, yuvarlanma direnci ve frenlerle tamamla (`AircraftGroundController`)
- [x] Y=90° / -Z ileri yönünde pist hızlanırken oluşan yaklaşık 14° eğilmeyi Ground Controller ve zemin temasıyla gider
- [x] 13 m/s rotasyon komutlu kontrollü kalkış senaryosunu oluştur ve FlightTest pistinde yaklaşık 17,2 m/s yerden kesilme referansını doğrula
- [x] Toplam hava hızı, dikey hız, signed angle of attack ve uçuş durumunu debug paneline ekle
- [ ] Propeller itki eğrisini gerçek araç verileriyle kalibre et; mevcut 1.000 N yalnızca prototip değeridir
- [x] Mevcut prototip drag modeliyle doğal uçuş hızını doğrula ve prototip hız zarfını belirle — 75 m/s caution, 85 m/s overspeed, 80 m/s recovery
- [ ] Prototip hız zarfını gerçek araç Vne ve işletme limitleriyle yeniden kalibre et
- [x] Sabit lift katsayısını açı-of-attack/stall modeliyle geliştir; maksimum güvenli hız ve gerçek araç kalibrasyonu açık kapsamdır
- [ ] Takip kamerasına arazi/geometri çarpışması ve görüş engeli yönetimi ekle
- [ ] Takip kamerası mesafe, yükseklik ve yumuşatma değerlerini uçuş fiziği tamamlandıktan sonra final kullanıcı testiyle kalibre et
- [ ] Yüksek RPM için pervane blur/disc görselleştirmesini değerlendir; mevcut çözüm dönüş hızını görsel örnekleme için ölçekler
- [ ] Açık Git klonunda çalıştırılabilir portföy sürümü için lisanslı Military Base Pack'e bağımlı `FlightTest` ortamına yeniden dağıtılabilir bir alternatif planla; mevcut sahne bu paketten 45 varlığa bağlıdır (2026-09-24 yerel ölçümü)

---

# SONRAKİ ÜÇ GÖREV

Bu bölüm her çalışma oturumunun sonunda güncellenmelidir.

1. Faz 14 için kaynakları doğrulanmış dört sesi Unity içinde dinle; motor döngüsünü RPM/throttle davranışına bağlayıp motor kapalı/rölanti/tam gaz geçişlerini ölç.
2. Rüzgâr döngüsünü hava hızına ve uyarı bipini uygun durumlara bağla; 12,4 saniyelik iniş kaydının temas mı, pistte koşu mu için uygun olduğunu dinleyerek kararlaştır.
3. Ses karışımını iki kamera modunda doğrula; ardından Faz 14 görsel geri bildirim, atmosfer ve post-processing kalemlerini değerlendir.
