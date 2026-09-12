# Working Rules — UAV Flight Simulator

**Son güncelleme:** 2026-09-11  
**Kapsam:** Unity proje kökünde açılan Codex oturumları

Bu dosyanın amacı Codex'in projeyi güvenli, kanıta dayalı ve tutarlı biçimde incelemesini ve geliştirmesini sağlamaktır.

---

## 1. Yeni Sohbetin İlk Aşaması Salt Okunurdur

Yeni bir Codex sohbetinde ilk görev, proje üzerinde değişiklik yapmak değil, mevcut durumu denetlemek ve raporlamaktır.

Codex ilk aşamada:

- `AGENTS.md` ile kök dokümantasyonun tamamını okur,
- Git çalışma ağacını, branch'i, remote'ları ve yakın commit geçmişini inceler,
- `.gitignore` ile izlenen üçüncü taraf yollarını kontrol eder,
- `ProjectVersion.txt`, ilgili `ProjectSettings` dosyaları ve `Packages/manifest.json` ile `packages-lock.json` dosyalarını inceler,
- `Assets/_Project/` altındaki klasörleri, C# dosyalarını, assembly definition ve testleri inceler,
- Input System assetlerini ve üretilmiş kod sınırlarını doğrular,
- sahne, prefab ve `.meta` ilişkilerini güvenli ölçüde inceler,
- `TODO`, `FIXME`, yinelenen/eski dosya ve dokümantasyon çelişkilerini arar,
- doğrulanan bulgular ile yalnızca belgelerde ileri sürülen bilgileri birbirinden ayırır.

İlk cevap yalnızca inceleme raporu ve öncelikli öneriler içerir. Bu aşamada dosya oluşturulmaz, değiştirilmez, silinmez, formatlanmaz veya taşınmaz. Codex uygulamaya geçmeden önce geliştiricinin açık onayını bekler.

---

## 2. Denetim Sonrası Yetki

Geliştirici raporu değerlendirdikten sonra açık ve somut bir görev verdiğinde Codex bu kapsam içinde proje dosyalarını düzenleyebilir.

Codex:

- mevcut dosyayı değiştirmeden önce inceler,
- kullanıcının alakasız değişikliklerini korur,
- küçük ve geri alınabilir yamaları tercih eder,
- değiştirdiği dosyaları ve gerekçelerini bildirir,
- mümkün olan güvenli derleme, test ve statik kontrolleri çalıştırır,
- yetki, veri veya kapsam engeli oluşursa durup kullanıcıdan yön ister.

Aşağıdaki işlemler ayrıca ve açıkça yetkilendirilmedikçe yapılmaz:

- `git push` veya force-push,
- branch ya da tag silme,
- geçmiş yeniden yazma,
- release veya dış yayın oluşturma,
- lisanslı üçüncü taraf içeriği repoya ekleme,
- geniş kapsamlı veya geri dönüşü zor silme işlemleri.

Unity'nin serileştirilmiş sahne/prefab dosyaları ve ikili assetleri özel dikkat gerektirir. Mümkünse Unity Editor veya Unity'ye uygun araçlar kullanılır; güvenli otomasyon mümkün değilse geliştiriciye kesin Editor adımları verilir.

---

## 3. İletişim Kuralı

Talimatlar açık, kısa ve doğrulanabilir olmalıdır.

Unity için gerektiğinde şunları belirt:

- kesin proje/dosya yolu,
- Unity menüsü,
- GameObject ve Component,
- Inspector alanı ve değer,
- beklenen sonuç ve doğrulama adımı.

Blender için gerektiğinde şunları belirt:

- Object/Edit veya ilgili mod,
- seçim türü,
- menü veya kısayol,
- sayısal değer,
- ekranda beklenen değişiklik.

Belirsizliği tahminle kapatma. Kanıt gerekiyorsa yalnızca ilgili çıktıyı veya ekran görüntüsünü iste.

---

## 4. Kodlama ve Mimari Kuralları

Namespace kökü:

```text
MertKaan.UAVSimulator
```

Temel ilkeler:

- Inspector ayarlarında mümkün olduğunda `[SerializeField] private` kullan.
- Girdi okuma ile fizik uygulamasını ayır.
- Rigidbody kuvvetlerini fixed-timestep yolunda uygula.
- Monolitik uçak kontrol sınıfı oluşturma.
- Sürekli çalışan pahalı sahne aramalarından kaçın.
- Magic number yerine anlamlı ayar alanları kullan.
- Mevcut davranışı görev gerektirmedikçe değiştirme.
- Yeni paket önermeden önce gerekliliğini, lisansını ve Unity sürümü uyumluluğunu açıkla.

Üretilmiş Input System wrapper dosyasını elle düzenleme:

```text
Assets/_Project/Scripts/Input/Generated/AircraftInputActions.cs
```

---

## 5. Proje Sabitleri

```text
Unity:               6000.3.20f1
Release:             Unity 6.3 LTS
Pipeline:            Universal Render Pipeline (URP)
Platform:            Windows 64-bit
Color Space:         Linear
Input:               Unity Input System
UI:                  uGUI + TextMeshPro
Physics:             Rigidbody tabanlı yarı gerçekçi
Cinemachine:         Kamera fazına kadar ertelendi
Asset Serialization: Force Text
Repository:          Flonq/uav-flight-simulator
```

Ana sahne:

```text
Assets/_Project/Scenes/FlightTest.unity
```

Asset inceleme sahnesi:

```text
Assets/_Project/Scenes/AssetReview.unity
```

Aşağıdakiler tarihsel referanstır; canlı repoda yeniden doğrulanmadan güncel kabul edilmez:

```text
Clean main baseline: 5a08b2662f49a18d737340703869104089507030
Known old branch:    feature/engine-throttle
```

---

## 6. Uçak Mimarisi ve Sorumluluk Sınırları

Korunacak temel ayrım:

```text
AircraftRoot
└── VisualPivot
```

- Fizik kökü uçuş durumunu taşır; görsel model `VisualPivot` altında onu takip eder.
- `AircraftInputReader` yalnızca ham kullanıcı komutlarını sağlar.
- `AircraftEngine`; throttle state/ramp, motor durumu, RPM, thrust hesabı ve propulsion kuvvetinden sorumludur.
- `AircraftPhysics`; lift, drag ve aerodinamik dönme kuvvetlerinden sorumludur.
- `AircraftGroundController`; fren ve yer hareketinden sorumludur.
- Input ve motor bileşenlerinde ayrı throttle state tutulmaz.

---

## 7. Özel İHA Modeli

Geliştiricinin 2026-09-11 tarihli son teyidine göre özel Blender İHA modeli modelleme açısından tamamlanmıştır. Model; gövde, ana kanat, çift tail boom, V-tail, pusher pervane, iniş takımları, ayrı aileronlar ve ayrı ruddervatorlar içerir.

Unity importunda ayrıca doğrulanacaklar:

- metre ölçeği ve transform değerleri,
- Unity yerel `+Z` burun yönü,
- aileron, ruddervator, pervane ve tekerlek pivot/yerel eksenleri,
- normals, tangents ve shading,
- materyal slotları ve URP uyumluluğu,
- triangle sayısı, hiyerarşi ve isimler,
- Blender referans/helper/cutter/guide nesnelerinin export dışında kalması,
- prefab referansları,
- mevcut collider ve Center of Mass uyumu,
- Input ve fizik prototipinin bozulmaması,
- `AssetReview` ve `FlightTest` sahnelerinde Console/Play Mode kontrolü.

Özel model doğrulanana kadar mevcut çalışan Unity görselini veya Meshy `SilentSentinel` deneyini silme ya da üzerine yazma.

---

## 8. Üçüncü Taraf İçerik ve Git Güvenliği

Military Base Pack'in beklenen yerel yolu:

```text
Assets/ThirdParty/Tiny Teacup Studio/Military Base Pack/
```

Bu paket lisans nedeniyle açık Git deposunda dağıtılmaz. Klasörü ve ilgili `.meta` dosyasını force-add etme; ignore kurallarını gerekçesiz değiştirme; paketi içeren eski Git geçmişini yeniden devreye sokma.

Çalışma ağacı kirliyse kullanıcı değişikliklerini koru. `git reset --hard`, `git checkout --`, geniş kapsamlı temizleme veya geçmiş yeniden yazma işlemlerini açık izin olmadan kullanma.

---

## 9. Test Beklentileri

Her anlamlı Unity değişikliğinden sonra uygun kontrolleri belirt ve mümkünse çalıştır:

- derleme/Console temizliği,
- sahne ve prefab referansları,
- Play Mode davranışı,
- physics ve input regresyonu,
- testler,
- import ve platform uyumluluğu.

Blender/FBX entegrasyonunda ayrıca obje sayısı, hiyerarşi, pivot, eksen, normals, materyal ve export filtrelerini doğrula.

Doğrulanamayan bir sonucu tamamlanmış gibi sunma; `doğrulandı`, `belgeye göre` ve `bilinmiyor` ifadelerini açıkça ayır.

---

## 10. Kaynak Önceliği

Bilgiler çelişirse şu sıra kullanılır:

```text
1. Geliştiricinin en yeni açık teyidi
2. Canlı repository ve güncel araç çıktısı
3. PROJECT_HANDOFF_2026-09-11.md
4. TECHNICAL_DECISIONS.md
5. TASKS.md
6. PROJECT_OVERVIEW.md
7. README.md
8. Eski tarihli handoff ve varsayımlar
```

Eski belgeler yararlı tarihsel bağlamdır; güncel repo kanıtının önüne geçmez.
