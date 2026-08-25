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
- [x] Unity sürümünü kesinleştir
- [x] Render Pipeline seçimini kesinleştir
- [x] UI teknolojisini kesinleştir
- [x] İHA modelini kesinleştir
- [ ] Proje takvimini oluştur

### Çıkış kriteri

Projenin teknik temeli ve ilk sürüm kapsamı açıkça tanımlanmış olmalıdır.

---

# FAZ 1 — Unity Proje Kurulumu

- [x] Yeni Unity projesini oluştur
- [x] Proje adını belirle
- [x] Windows hedef platformunu ayarla
- [x] Renk uzayını kontrol et
- [x] Input System paketini kur
- [x] TextMeshPro temel kaynaklarını ekle
- [x] Gerekliyse Cinemachine paketini kur
  - İlk prototip için gerekli görülmedi; Faz 8 sırasında tekrar değerlendirilecek.
- [x] `_Project` ana klasörünü oluştur
- [x] Önerilen alt klasör yapısını oluştur
- [x] İlk test sahnesini oluştur
- [x] Sahneyi `FlightTest` adıyla kaydet
- [x] Proje ayarlarının ilk yedeğini al
- [x] Git deposunu başlat
- [x] Unity için `.gitignore` ekle
- [x] İlk commit'i oluştur
- [x] GitHub reposunu oluştur
- [x] Yerel repoyu GitHub'a push et

### Çıkış kriteri

Proje hatasız açılmalı, boş test sahnesi çalışmalı ve GitHub üzerinde ilk sürüm bulunmalıdır.

---

# FAZ 2 — Test Ortamı ve Havaalanı

- [x] Test zemini ve havaalanı çevresini hazırla
  - Military Base Pack test ortamının temel çevresi olarak kullanılıyor.
- [x] Pist / test yüzeyini hazırla
- [x] Pist collider davranışını Play Mode'da doğrula
- [x] İHA spawn noktası oluştur
- [x] Hangar ve üs çevresini ekle
- [x] Yer kontrol istasyonu alanını belirle
- [x] Directional Light temel ayarlarını doğrula
- [x] Skybox kullanımını değerlendir
- [x] Sis kullanımını değerlendir
- [x] Test kamerasını doğrula
- [x] Çevre ölçeğini İHA ile karşılaştırarak doğrula
- [x] Havaalanı sahnesinde performans testi yap
- [x] Military Base Pack lisansını kontrol et
- [x] Military Base Pack URP materyal uyumluluğunu düzelt
- [x] Gereksiz Spot Light gölgelerini kapat
- [x] Play Mode testinde Console'u hatasız ve uyarısız doğrula

### Çıkış kriteri

İHA fizik testleri için ölçeği doğru ve performanslı bir test sahası hazırlanmış olmalıdır.

---

# FAZ 3 — İHA Modeli ve Fizik Kökü

- [x] İHA modelini projeye aktar
- [x] Modelin lisansını belgeleyerek kontrol et
- [x] Model ölçeğini metre birimine göre ayarla
- [x] Pivot noktasını kontrol et
- [x] Model yönünü Unity eksenlerine göre düzelt
- [x] Fizik kök nesnesi oluştur
- [x] Görsel modeli fizik kökünün altına yerleştir
- [x] Rigidbody ekle
- [x] Kütle değerini belirle
- [x] Center of Mass ayarını kontrol et
  - Rigidbody Automatic Center Of Mass kullanılıyor; özel CoM ayarı uçuş fiziği geliştirilirken gerekirse yeniden değerlendirilecek.
- [x] Ana collider yapısını oluştur
- [x] Kanat colliderlarını değerlendir
- [x] Tekerlek veya iniş takımı colliderlarını oluştur
- [x] Prefab oluştur
- [x] Prefabı test sahnesine ekle
- [x] Yerçekimi ve pist temas testini gerçekleştir

### Çıkış kriteri

İHA modeli sahnede doğru ölçekte, doğru yönde ve fizik bileşenleriyle hazır bulunmalıdır.

---

# FAZ 4 — Girdi Sistemi

- [ ] `Aircraft.inputactions` dosyasını oluştur
- [ ] Pitch action oluştur
- [ ] Roll action oluştur
- [ ] Yaw action oluştur
- [ ] Throttle action oluştur
- [ ] Brake action oluştur
- [ ] Kamera değiştirme action oluştur
- [ ] EO kamera zoom action oluştur
- [ ] Pause action oluştur
- [ ] C# sınıf üretimini etkinleştir
- [ ] `AircraftInputReader` scriptini oluştur
- [ ] Girdi değerlerini debug panelinde göster
- [ ] Klavye kontrolünü test et
- [ ] Fare kontrolü gerekip gerekmediğini değerlendir
- [ ] Gamepad desteğini test et
- [ ] Joystick desteğini sonraki sürüm için değerlendir

### Çıkış kriteri

Bütün kullanıcı komutları fizik sisteminden bağımsız şekilde okunabilmelidir.

---

# FAZ 5 — Motor ve Throttle Sistemi

- [ ] `AircraftEngine` scriptini oluştur
- [ ] Minimum throttle tanımla
- [ ] Maksimum throttle tanımla
- [ ] Throttle artış ve azalış hızını belirle
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

---

# SONRAKİ ÜÇ GÖREV

Bu bölüm her çalışma oturumunun sonunda güncellenmelidir.

1. Test ortamındaki Directional Light, Skybox, sis ve yer kontrol istasyonu alanı kararlarını tamamla
2. İHA modelinin lisansını belgeleyerek kontrol et ve Center of Mass ayarını doğrula
3. Faz 2 ve Faz 3 dokümantasyonunu tamamlayıp test ortamı branch'ini kapat
