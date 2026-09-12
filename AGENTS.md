# AGENTS.md — UAV Flight Simulator

Bu talimatlar repository'nin tamamı için geçerlidir.

## Zorunlu İlk Oturum Protokolü

Yeni bir sohbette önce `PROJECT_HANDOFF_2026-09-12.md`, `WORKING_RULES_UAV_SIMULATOR.md`, `TECHNICAL_DECISIONS.md`, `TASKS.md`, `PROJECT_OVERVIEW.md` ve `README.md` dosyalarını tamamen oku.

Ardından projeyi salt okunur incele. İlk rapor tamamlanana ve kullanıcı açıkça uygulamaya geçmeni isteyene kadar:

- dosya oluşturma, değiştirme, silme, taşıma veya formatlama,
- Unity assetlerini yeniden serialize etme,
- paket ekleme/güncelleme,
- Git commit/push/checkout/reset/clean/rebase işlemi yapma.

Salt okunur denetimde en az şunları incele:

- Git status, branch, upstream, remote ve yakın commit geçmişi,
- `.gitignore`, tracked/untracked/ignored üçüncü taraf dosyalar,
- `ProjectSettings/ProjectVersion.txt` ve ilgili Unity proje ayarları,
- `Packages/manifest.json` ve `Packages/packages-lock.json`,
- `Assets/_Project/` klasör ağacı,
- tüm C# dosyaları, asmdef ve testler,
- Input System assetleri ve generated wrapper sınırı,
- sahne/prefab/metafile ilişkileri,
- `TODO`, `FIXME`, `HACK`, yinelenen/eski dosyalar,
- kök dokümantasyon ile canlı proje arasındaki çelişkiler.

İlk raporu şu başlıklarla sun:

1. Yönetici özeti
2. Repository ve Git durumu
3. Unity sürümü, pipeline, ayarlar ve paketler
4. Assets, sahneler ve prefablar
5. Kod ve mimari
6. Özel İHA modeli entegrasyon hazırlığı
7. Dokümantasyon farkları
8. Riskler ve teknik borçlar
9. Önceliklendirilmiş sonraki adımlar
10. İncelenemeyen noktalar

Bulguları **Doğrulandı**, **Belgeye göre** veya **Bilinmiyor** olarak ayır. Raporun sonunda kullanıcıdan hangi öneriye geçileceğini seçmesini bekle.

## Onay Sonrası Çalışma

Kullanıcı somut bir görev verdiğinde yalnızca o kapsamda değişiklik yap.

- Kirli çalışma ağacında kullanıcı değişikliklerini koru.
- Dosyayı düzenlemeden önce mevcut içeriği incele.
- Küçük, geri alınabilir ve amaç odaklı yamalar kullan.
- Alakasız biçimlendirme veya refactor yapma.
- Değişen dosyaları ve nedenlerini bildir.
- Uygun derleme, test ve statik kontrolleri çalıştır.
- Unity Editor içinde doğrulanamayan sonucu tamamlanmış gibi sunma.
- Push, force-push, branch/tag silme, geçmiş yeniden yazma ve release işlemleri için ayrıca açık izin iste.

## Proje Sabitleri

```text
Unity:               6000.3.20f1 / Unity 6.3 LTS
Pipeline:            URP
Platform:            Windows 64-bit
Color Space:         Linear
Input:               Unity Input System
UI:                  uGUI + TextMeshPro
Serialization:       Force Text
Namespace:           MertKaan.UAVSimulator
Main Scene:          Assets/_Project/Scenes/FlightTest.unity
Asset Review Scene:  Assets/_Project/Scenes/AssetReview.unity
```

Canlı dosyalar bunlarla çelişirse önce farkı raporla; kendiliğinden ayar değiştirme.

## Korunacak Mimari

```text
AircraftRoot
└── VisualPivot
```

- Input yalnızca komut sağlar.
- `AircraftEngine` throttle, RPM ve propulsion sahibidir.
- `AircraftPhysics` lift, drag ve aerodinamik tork sahibidir.
- `AircraftGroundController` fren ve yer hareketi sahibidir.
- Rigidbody kuvvetleri fixed-timestep yolunda uygulanır.
- `AircraftInputReader` ve `AircraftEngine` aynı throttle state'i ayrı ayrı sahiplenmez.

Şu generated dosyayı elle düzenleme:

```text
Assets/_Project/Scripts/Input/Generated/AircraftInputActions.cs
```

## Asset ve Lisans Güvenliği

Military Base Pack yalnızca yerel bağımlılıktır ve açık Git deposuna eklenmemelidir:

```text
Assets/ThirdParty/Tiny Teacup Studio/Military Base Pack/
```

Unity'de doğrulanan özel Blender İHA modelini, prefablarını veya elle ayarlanmış collider'larını yeniden oluşturma işlemiyle ezme. Meshy yedeğini ve eski pasif uçak instance'larını final uçuş sistemleri doğrulanmadan silme.

Ayrıntılı kurallar için `WORKING_RULES_UAV_SIMULATOR.md` dosyasını esas al.
