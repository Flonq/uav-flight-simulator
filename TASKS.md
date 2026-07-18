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
- [ ] Unity sürümünü kesinleştir
- [ ] Render Pipeline seçimini kesinleştir
- [ ] UI teknolojisini kesinleştir
- [ ] İHA modelini kesinleştir
- [ ] Proje takvimini oluştur

### Çıkış kriteri

Projenin teknik temeli ve ilk sürüm kapsamı açıkça tanımlanmış olmalıdır.

---

# FAZ 1 — Unity Proje Kurulumu

- [ ] Yeni Unity projesini oluştur
- [ ] Proje adını belirle
- [ ] Windows hedef platformunu ayarla
- [ ] Renk uzayını kontrol et
- [ ] Input System paketini kur
- [ ] TextMeshPro temel kaynaklarını ekle
- [ ] Gerekliyse Cinemachine paketini kur
- [ ] `_Project` ana klasörünü oluştur
- [ ] Önerilen alt klasör yapısını oluştur
- [ ] İlk test sahnesini oluştur
- [ ] Sahneyi `FlightTest` adıyla kaydet
- [ ] Proje ayarlarının ilk yedeğini al
- [ ] Git deposunu başlat
- [ ] Unity için `.gitignore` ekle
- [ ] İlk commit'i oluştur
- [ ] GitHub reposunu oluştur
- [ ] Yerel repoyu GitHub'a push et

### Çıkış kriteri

Proje hatasız açılmalı, boş test sahnesi çalışmalı ve GitHub üzerinde ilk sürüm bulunmalıdır.

---

# FAZ 2 — Test Ortamı ve Havaalanı

- [ ] Basit zemin oluştur
- [ ] Pist modeli veya geçici pist oluştur
- [ ] Pist collider ayarlarını yap
- [ ] Başlangıç noktası oluştur
- [ ] İHA spawn noktası oluştur
- [ ] Basit hangar alanı ekle
- [ ] Yer kontrol istasyonu alanı ekle
- [ ] Directional Light ayarla
- [ ] Skybox ayarla
- [ ] Sis ayarlarını değerlendir
- [ ] Test kamerası oluştur
- [ ] Çevre ölçeğini metre birimine göre kontrol et
- [ ] Havaalanı sahnesinde 60 FPS testi yap

### Çıkış kriteri

İHA fizik testleri için ölçeği doğru ve performanslı bir test sahası hazırlanmış olmalıdır.

---

# FAZ 3 — İHA Modeli ve Fizik Kökü

- [ ] İHA modelini projeye aktar
- [ ] Modelin lisansını belgeleyerek kontrol et
- [ ] Model ölçeğini metre birimine göre ayarla
- [ ] Pivot noktasını kontrol et
- [ ] Model yönünü Unity eksenlerine göre düzelt
- [ ] Fizik kök nesnesi oluştur
- [ ] Görsel modeli fizik kökünün altına yerleştir
- [ ] Rigidbody ekle
- [ ] Kütle değerini belirle
- [ ] Center of Mass ayarını kontrol et
- [ ] Ana collider yapısını oluştur
- [ ] Kanat colliderlarını değerlendir
- [ ] Tekerlek veya iniş takımı colliderlarını oluştur
- [ ] Prefab oluştur
- [ ] Prefabı test sahnesine ekle

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

1. Unity sürümünü kesinleştir
2. Yeni Unity projesini oluştur
3. Git ve klasör yapısını hazırla
