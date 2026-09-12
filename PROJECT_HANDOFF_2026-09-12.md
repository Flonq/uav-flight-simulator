# Project Handoff — UAV Flight Simulator

**Tarih:** 2026-09-12

**Durum:** Özel İHA Unity entegrasyonu ve ilk görsel kontrol yüzeyi testi tamamlandı

**Proje kökü:** `C:\Users\mertk\Desktop\Projeler\Unity Projects\UAVFlightSimulator`

Bu belge canlı repository denetimi, Unity Editor doğrulamaları ve geliştiricinin 2026-09-12 tarihli test sonuçlarına dayanır. Önceki handoff belgeleri tarihsel kayıttır.

---

## 1. Güncel Sonuç

Blender 5.2.1 LTS ile geliştirilen özel İHA modeli ayrı bir Unity entegrasyon alanına aktarıldı. Model, eski Meshy görseli silinmeden yeni bir görsel prefab ve yeni bir fizik prototipi üzerinden `FlightTest` sahnesine yerleştirildi.

Doğrulanan ana sonuçlar:

- Unity `6000.3.20f1`, URP, Windows 64-bit, Linear ve Force Text yapılandırması korundu.
- Özel model 33 renderer/benzersiz mesh ve 44.922 triangle içeriyor.
- Görsel bounds yaklaşık `(12.14, 1.84, 6.79)` metredir.
- Tek URP/Lit materyal ile base color, metallic-smoothness ve normal dokuları bağlandı.
- `AircraftRoot > VisualPivot` mimarisi korundu.
- Yeni fizik prototipinde root üzerinde tek Rigidbody ve 10 adet elle ayarlanmış primitive compound collider bulunuyor.
- Collider'ların hiçbiri trigger değil; çocuklarda ek Rigidbody yok.
- Play Mode testinde uçak sıçramadan, savrulmadan veya zemine gömülmeden üç tekerlek üzerinde kararlı durdu.
- Aileron ve ruddervator pivotları Unity yerel X ekseninde; `Rotor_Pivot` Unity yerel Y ekseninde kaymadan dönüyor.
- `AircraftControlSurfaceAnimator` ile roll/pitch/yaw görsel yüzey hareketleri çalıştı.
- Input Debug panelinin eski pasif reader referansı için aktif-reader çözümlemesi eklendi; klavye inputu ve animasyonlar Console hatası olmadan doğrulandı.

---

## 2. Eklenen veya Güncellenen Ana Dosyalar

```text
Assets/_Project/Art/Aircraft/CustomUAV/
Assets/_Project/Art/Materials/M_CustomUAV_URP.mat
Assets/_Project/Prefabs/Aircraft/PF_CustomUAVVisual.prefab
Assets/_Project/Prefabs/Aircraft/PF_CustomUAVAircraftPrototype.prefab
Assets/_Project/Editor/CustomUAVIntegrationTool.cs
Assets/_Project/Scripts/Aircraft/AircraftControlSurfaceAnimator.cs
Assets/_Project/Scripts/UI/Debug/InputDebugPanel.cs
Assets/_Project/Scenes/FlightTest.unity
```

Kaynak Blender çalışma alanı Unity repository'sinin dışındadır:

```text
C:\Users\mertk\Desktop\Projeler\UAVFlightSimulator_Blender\CustomUAV
```

Unity'ye alınan runtime FBX ve dokular, bu kaynak alandaki doğrulanmış export paketinden kopyalanmıştır. `.blend`, helper/cutter/guide ve Blender çalışma dosyaları Git kapsamına alınmamıştır.

---

## 3. Sahne ve Prefab Durumu

`FlightTest` içinde:

- yeni özel UAV prefab instance'ı aktif `AircraftRoot` adını taşır,
- önceki aktif Meshy uçak `AircraftRoot_Meshy_Backup` adıyla pasiftir,
- daha önce de pasif olan `AircraftRoot (1)` ve `AircraftRoot (2)` henüz silinmemiştir,
- yeni root üzerinde `AircraftInputReader` ve test edilen görsel kontrol yüzeyi bileşeni bulunur,
- `InputDebugPanel` aktif uçak reader'ını kullanmalıdır.

`PF_CustomUAVAircraftPrototype.prefab` mevcutsa `Build or Update Aircraft Prototype` komutu artık yeniden oluşturma yapmaz; yapıyı doğrular ve elle ayarlanmış collider/bileşenleri korur.

---

## 4. Hareketli Parça Eksenleri

```text
Aileron_Left:       local X; + açı arka kenarı yukarı taşır
Aileron_Right:      local X; + açı arka kenarı yukarı taşır
Ruddervator_Left:   local X; + açı yüzeyi yukarı taşır
Ruddervator_Right:  local X; + açı yüzeyi yukarı taşır
Rotor_Pivot:        local Y; + yönde saat yönünün tersine döner
```

`AircraftControlSurfaceAnimator` yalnızca görsel yüzeyleri hareket ettirir. Rigidbody kuvveti uygulamaz, throttle state'i sahiplenmez ve pervaneyi döndürmez. Pervane devri gelecekte `AircraftEngine` RPM çıktısına bağlanmalıdır.

---

## 5. Korunacak Mimari

```text
AircraftRoot
└── VisualPivot
```

| Bileşen | Sorumluluk |
|---|---|
| `AircraftInputReader` | Ham pitch/roll/yaw/throttle-step/brake/kamera/UI komutları |
| `AircraftControlSurfaceAnimator` | Komutların görsel aileron/ruddervator hareketine dönüştürülmesi |
| `AircraftEngine` | Throttle state/ramp, motor durumu, RPM, thrust ve propulsion |
| `AircraftPhysics` | Lift, drag ve aerodinamik torklar |
| `AircraftGroundController` | Fren, yerde olma ve yer hareketi |

Generated `Assets/_Project/Scripts/Input/Generated/AircraftInputActions.cs` dosyası elle düzenlenmez.

---

## 6. Bilinen Geçici Değerler ve Teknik Borç

- Rigidbody mass değeri hâlâ prototip `100` değeridir.
- `Automatic Center of Mass` ve `Automatic Tensor` açıktır. Yer testi kararlı olsa da gerçek kütle/CG verisi bulunmadığı için fiziksel doğruluk iddia edilmez.
- `AircraftInputReader` halen throttle state/ramp tutuyor; Phase 5 öncesinde sahiplik `AircraftEngine` bileşenine taşınmalıdır.
- `AircraftEngine`, propulsion, aerodinamik uçuş fiziği ve yer kontrolü henüz uygulanmadı.
- Kontrol yüzeyi bileşeninin final prefab yerleşimi ve sahne override temizliği tamamlanmalıdır.
- `AssetReview` için ayrıca kaydedilmiş final smoke testi kanıtı eksiktir.
- Eski Meshy yedeği ve iki eski pasif uçak, final kabul öncesinde bilinçli olarak korunuyor.
- Test/asmdef yapısı henüz yoktur.
- 4096 dokular ve LOD gereksinimi profiler/build aşamasında yeniden değerlendirilmelidir.

---

## 7. Üçüncü Taraf Güvenliği

Military Base Pack yalnızca yerel bağımlılıktır:

```text
Assets/ThirdParty/Tiny Teacup Studio/Military Base Pack/
```

Bu yol ve ilgili `.meta` Git tarafından yok sayılmaya devam eder. ERP, Library, Temp, Logs, UserSettings ve IDE çıktıları da commit kapsamına alınmaz.

---

## 8. Önceliklendirilmiş Sonraki Adımlar

1. `AircraftControlSurfaceAnimator` ve doğru Input Debug referansını prefab/sahne sınırında kalıcılaştır; duplicate component bırakma.
2. `AircraftInputReader`ı yalnızca komut üretecek şekilde düzenleyip throttle state/ramp sahipliğini yeni `AircraftEngine` bileşenine taşı.
3. `AircraftEngine` ile RPM ve propulsion üret; `Rotor_Pivot` görsel dönüşünü engine RPM çıkışına bağla.
4. Basit pist hızlanma ve fren testinden sonra `AircraftPhysics` lift/drag/tork katmanına geç.
5. `AssetReview` ve `FlightTest` final smoke testlerini kaydet.
6. Yeni sistemler kabul edildikten sonra Meshy yedeği ile açıklanamayan eski pasif uçak instance'larını kontrollü biçimde temizle.

---

## 9. Git Durumu

Entegrasyon çalışması `codex/custom-uav-integration` branch'inde yürütülmüştür. Push veya yeni çalışma öncesinde canlı `git status`, upstream ve remote yeniden doğrulanmalıdır.
