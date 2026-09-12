# Project Handoff — UAV Flight Simulator

**Tarih:** 2026-09-11  
**Durum:** Yeni Codex sohbeti için güncel devir özeti  
**Proje kökü:** `C:\Users\mertk\Desktop\Projeler\Unity Projects\UAVFlightSimulator`

> Bu belge, 2026-09-11 tarihine kadar geliştiricinin verdiği son teyitleri ve önceki çalışma kayıtlarını birleştirir. Canlı repository denetimi yapılmadan dosya, branch, sahne veya paketlerin hâlâ aynı olduğu varsayılmamalıdır.

---

## 1. İlk Yapılacak İş

Yeni Codex sohbeti projede hiçbir değişiklik yapmadan önce tüm repository'yi ayrıntılı ve salt okunur incelemelidir.

İlk aşamanın çıktısı:

1. yönetici özeti,
2. Git/repository durumu,
3. Unity sürümü, render pipeline, paketler ve proje ayarları,
4. `Assets/_Project/` yapısı, sahneler ve prefablar,
5. C# kodu, mimari ve Input System durumu,
6. özel İHA modelinin entegrasyon hazırlığı,
7. dokümantasyon ile canlı proje arasındaki farklar,
8. riskler ve teknik borçlar,
9. önceliklendirilmiş sonraki adımlar,
10. incelenemeyen veya Unity Editor gerektiren noktalar.

Rapor sunulmadan ve geliştirici açıkça onay vermeden dosya değiştirilmemelidir.

---

## 2. Projenin Amacı ve Kapsamı

UAV Flight Simulator, Unity ve C# ile geliştirilen Windows masaüstü tabanlı sabit kanatlı İHA uçuş simülatörü ve portföy projesidir.

MVP hedefi:

- tek bir özel sabit kanatlı İHA,
- tek bir havaalanı/test sahası,
- klavye/fare kontrolü,
- Rigidbody tabanlı yarı gerçekçi uçuş,
- pist hareketi, kalkış ve iniş,
- takip ve EO kamera,
- temel telemetri,
- en az üç waypoint içeren görev,
- görev sonucu,
- Windows 64-bit build.

Bu çalışma resmî bir Baykar ürünü değildir ve sertifikalı uçuş eğitim simülatörü olmayı hedeflemez.

---

## 3. Bilinen Teknik Temel

Önceki oturumlarda doğrulanan ve canlı projede yeniden denetlenecek değerler:

```text
Unity Editor:        6000.3.20f1 / Unity 6.3 LTS
Render Pipeline:     Universal Render Pipeline (URP)
Target Platform:     Windows 64-bit
Color Space:         Linear
Input:               Unity Input System
UI:                  uGUI + TextMeshPro
Physics:             Rigidbody tabanlı yarı gerçekçi
Asset Serialization: Force Text
Namespace:           MertKaan.UAVSimulator
Repository:          Flonq/uav-flight-simulator
```

Ana sahneler:

```text
Assets/_Project/Scenes/FlightTest.unity
Assets/_Project/Scenes/AssetReview.unity
```

Üretilmiş Input System kodu:

```text
Assets/_Project/Scripts/Input/Generated/AircraftInputActions.cs
```

Bu üretilmiş dosya elle düzenlenmemelidir.

---

## 4. Bilinen Tamamlanmış Çalışmalar

### Unity proje temeli

- Unity projesi, URP, Windows 64-bit ve Linear color space yapılandırıldı.
- uGUI ve TextMeshPro kullanımı kesinleştirildi.
- Force Text asset serialization kullanılıyor.
- `_Project` tabanlı proje klasör yapısı oluşturuldu.
- `FlightTest` ve `AssetReview` sahneleri oluşturuldu.

### Test ortamı

- Tiny Teacup Studio Military Base Pack ile yerel test havaalanı hazırlandı.
- Pist/zemin teması, spawn alanı, ışıklandırma ve temel ortam çalıştı.
- Military Base Pack materyalleri URP/Lit ile uyumlu hâle getirildi.
- 53 Spot Light için gölgeler kapatıldı.
- Önceki smoke testte Environment, Aircraft ve Input çalıştı; Console temizdi.

### Fizik ve görsel prototipi

- `AircraftRoot > VisualPivot` ayrımı oluşturuldu.
- Rigidbody prototipi kullanıldı: mass `100`, gravity açık, interpolate açık, continuous collision ve automatic center of mass.
- Geçici gövde, kanat ve tekerlek collider prototipleri oluşturuldu.
- `PF_AircraftVisualPrototype` ve `PF_AircraftPrototype` prefab prototipleri oluşturuldu.
- Önceki görsel/fizik prototipi özel model doğrulanana kadar korunmalıdır.

### Input System

- Aircraft Input Actions oluşturuldu.
- Pitch, roll, yaw, throttle, brake, camera switch, EO zoom ve pause girdileri tanımlandı.
- `AircraftInputReader` oluşturuldu.
- Pasif input debug paneli tamamlandı.
- Klavye/fare ve genel `<Gamepad>` bindingleri üzerinden DualSense test edildi.
- Joystick/HOTAS desteği MVP sonrasına ertelendi.

### Özel Blender İHA modeli

Geliştiricinin son açık teyidine göre modelleme tamamlandı. Model şunları içeriyor:

- fuselage,
- ana kanat,
- çift tail boom,
- V-tail,
- pusher motor/fairing/pervane,
- iniş takımları,
- ayrı sağ/sol aileron,
- ayrı sağ/sol ruddervator.

Aileron ve ruddervatorlardaki önceki geometri/yerleşim sorunları sonradan düzeltilmiştir. Eski ekran görüntüleri veya ara mesh audit değerleri final modelin güncel durumu olarak kullanılmamalıdır.

---

## 5. Henüz Tamamlandığı Doğrulanmamış Çalışmalar

### Özel modelin Unity entegrasyonu

- temiz export kopyası ve export hiyerarşisi,
- helper/reference/cutter/guide nesnelerinin dışarıda bırakılması,
- Unity importu,
- metre ölçeği,
- yerel `+Z` burun yönü,
- hareketli parça pivotları ve yerel eksenleri,
- normals/tangents/shading,
- materyal slotları ve URP materyalleri,
- triangle sayısı ve gerekirse LOD,
- `VisualPivot` entegrasyonu,
- collider ve Center of Mass uyarlaması,
- final prefab ve sahne doğrulaması.

### Simülasyon sistemleri

- `AircraftEngine` ve gerçek thrust,
- throttle sahipliği/rampası refactor'u,
- lift, drag, aerodinamik torklar ve stall,
- tam yer hareketi ve fren,
- kalkış ve iniş,
- kamera modları ve EO kamera,
- telemetri,
- waypoint/görev sistemi,
- yer kontrol istasyonu UI,
- ses sistemi,
- performans profili ve Windows build.

Bu maddeler canlı repository kanıtı olmadan tamamlanmış sayılmamalıdır.

---

## 6. Uçak Mimarisinin Korunacak Sınırları

```text
AircraftRoot
└── VisualPivot
```

- `AircraftRoot` fizik ve simülasyon köküdür.
- Özel mesh/hareketli görsel parçalar `VisualPivot` altında bulunmalıdır.
- Görsel model fizik kökünü sürmemelidir.
- Yeni görsel doğrulanmadan eski çalışan görsel silinmemelidir.

Kod sorumlulukları:

| Bileşen | Sorumluluk |
|---|---|
| `AircraftInputReader` | Ham kullanıcı girdisi |
| `AircraftEngine` | Throttle state/ramp, motor durumu, RPM, thrust ve propulsion |
| `AircraftPhysics` | Lift, drag ve aerodinamik torklar |
| `AircraftGroundController` | Fren, yerde olma ve yer hareketi |
| `AircraftTelemetry` | Uçuş verilerinin okunması ve yayınlanması |

`AircraftInputReader` ile `AircraftEngine` aynı throttle state'in iki ayrı sahibi olmamalıdır. Bu refactor Phase 5 başlamadan önce doğrulanmalıdır.

---

## 7. Özel Model İçin Önerilen Güvenli Entegrasyon Sırası

1. Canlı repo denetimini ve raporu tamamla.
2. Çalışma ağacındaki kullanıcı değişikliklerini belirle ve koru.
3. Blender'da yalnızca gerekli mesh/armature/empty nesnelerini içeren export kopyası hazırla.
4. Modeli ayrı bir entegrasyon alanına aktar; mevcut görselin üzerine yazma.
5. Önce `AssetReview` sahnesinde ölçek, eksen, pivot, normals, materyal ve hiyerarşiyi doğrula.
6. Kontrol yüzeyleri ile pervaneyi küçük test rotasyonlarıyla doğrula.
7. `VisualPivot` altında geçici bağla; eski görseli silmeden devre dışı bırakılabilir alternatif olarak tut.
8. Collider ve Center of Mass değerlerini görselden bağımsız fakat yeni boyutlara uygun biçimde değerlendir.
9. `FlightTest` sahnesinde input ve physics regresyon kontrolü yap.
10. Console temizliği ve prefab referans kontrolünden sonra final entegrasyon kararı ver.

Önerilen import alanı, canlı klasör yapısıyla uyuştuğu doğrulandıktan sonra:

```text
Assets/_Project/Art/Aircraft/CustomUAV/
```

---

## 8. Git ve Üçüncü Taraf İçerik Güvenliği

Military Base Pack'in yerel yolu:

```text
Assets/ThirdParty/Tiny Teacup Studio/Military Base Pack/
```

Paket lisans nedeniyle açık repository içinde dağıtılmamalıdır. Klasör ve ilgili `.meta` dosyası ignore kapsamında kalmalıdır.

Git geçmişi daha önce bu asseti kaldırmak için temizlenmiştir. Aşağıdaki bilgiler yalnızca tarihsel başlangıç noktasıdır ve canlı repo denetiminde doğrulanmalıdır:

```text
Clean main baseline: 5a08b2662f49a18d737340703869104089507030
Known old branch:    feature/engine-throttle
```

Codex açık talimat olmadan push, force-push, branch silme veya geçmiş yeniden yazma yapmamalıdır.

---

## 9. İlk Codex Denetim Kontrol Listesi

### Repository ve Git

- `git status --short --branch`
- mevcut branch, upstream, remote URL'leri ve yakın log
- tracked/untracked/ignored dosyalar
- büyük dosya veya yanlışlıkla izlenen üçüncü taraf asset
- `.gitignore` kapsamı
- eski/çoğaltılmış dokümantasyon

### Unity yapılandırması

- `ProjectSettings/ProjectVersion.txt`
- Graphics/Quality/Player/Editor ayarlarının ilgili bölümleri
- aktif render pipeline assetleri
- `Packages/manifest.json` ve `Packages/packages-lock.json`
- Input System ve UI paketleri
- build target ve sahne listesi kanıtı

### Assets ve sahneler

- `Assets/_Project/` klasör ağacı
- sahne ve prefab dosyaları ile `.meta` bütünlüğü
- `AircraftRoot`, `VisualPivot` ve Aircraft instance'ları
- aktif/inactive instance'ların gerekçesi
- Military Base Pack bağımlılıkları
- missing script/material/reference işaretleri

### Kod ve mimari

- tüm C# dosyaları ve namespace tutarlılığı
- asmdef ve test yapısı
- `AircraftInputReader` gerçek uygulaması
- generated Input wrapper sınırı
- throttle sorumluluğu
- kullanılmayan, yinelenen veya yarım scriptler
- `TODO`, `FIXME`, `HACK`
- editör veya platform uyumluluğu riskleri

### Raporlama dili

Her önemli bulgu şu etiketlerden biriyle sunulmalıdır:

- **Doğrulandı:** canlı dosya veya araç çıktısıyla kanıtlandı.
- **Belgeye göre:** yalnızca bu dokümantasyonda yazıyor.
- **Bilinmiyor:** mevcut ortamdan doğrulanamadı.

---

## 10. Bilinen Belirsizlikler

Yeni Codex şu konuları özellikle yeniden doğrulamalıdır:

- güncel branch, HEAD ve çalışma ağacı,
- Unity klasör yapısının belgelerle birebir uyumu,
- paketlerin güncel sürümleri,
- `FlightTest` içindeki Aircraft instance sayısı ve inactive instance'ların amacı,
- InputReader'ın güncel throttle uygulaması,
- final Blender/FBX dosya adı ve export yöntemi,
- final modelin triangle sayısı, materyalleri ve pivotları,
- Unity Editor Console ve Play Mode'un güncel durumu.

---

## 11. Belge Önceliği

Çelişkide kullanılacak sıra:

```text
1. Geliştiricinin en yeni açık teyidi
2. Canlı repository ve güncel araç çıktısı
3. Bu handoff belgesi
4. TECHNICAL_DECISIONS.md
5. TASKS.md
6. PROJECT_OVERVIEW.md
7. README.md
8. 2026-09-06 handoff'u ve eski varsayımlar
```

`PROJECT_HANDOFF_2026-09-06.md` tarihsel bağlamdır; güncel durum kaynağı değildir.

---

## 12. Yeni Oturumun Beklenen İlk Sonucu

Yeni oturumun ilk cevabı kod veya dosya değişikliği değil, projenin gerçekten bulunduğu durumu kanıtlarıyla anlatan ayrıntılı bir rapor olmalıdır. Geliştirici bu raporu onayladıktan sonra önerilen ilk uygulama işi özel İHA modelinin güvenli Unity importu ve `VisualPivot` entegrasyonudur.
