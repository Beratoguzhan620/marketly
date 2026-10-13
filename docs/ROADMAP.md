# Marketly — Yol Haritası

Satıcı başvurusu, sepet, sipariş ve yorum onayı olan çok satıcılı e-ticaret platformu + admin paneli + dosya API'si.

- **Önerilen repo adı:** `marketly`
- **Teknoloji:** ASP.NET Core MVC (.NET 10), EF Core, SQL Server, Bootstrap 5 + özel SCSS, xUnit
- **Kurallar:** `CONTRIBUTING.md` · **Tasarım:** `docs/DESIGN.md`

Her aşama ayrı bir branch ve PR olarak yapılır (`stage/<no>-<kisa-ad>`). Bir aşama ancak tüm maddeleri bitince, build/testler geçince ve ekran görüntüleri eklenince tamamlanmış sayılır. Biten maddeler `[x]` yapılır.

## Aşama 1 — Altyapı ve temizlik
- [x] Proje adını "Marketly" yap (solution, namespace'ler `App.*` kalabilir ama başlıklar/marka Marketly)
- [x] .NET 8 → .NET 10'a yükselt; paketleri güncelle; build uyarılarını temizle
- [x] `EnsureCreated` yerine EF Core migrations; ilk migration + seed (kategoriler, 40+ ürün, görseller, kullanıcılar, siparişler, blog yazıları)
- [x] Kullanıcı şifreleri düz metin yerine hash'lenerek saklanır (seed'deki "1234" şifreleri dahil); giriş akışı buna göre güncellenir
- [x] API key / connection string'ler user-secrets / env; `appsettings.json` içinde gerçek değer yok
- [x] Hazır temaların (SB Admin 2, Ogani) lisans/kredi durumu kontrolü — kullanılmayan vendor dosyalarını sil (`docs/THIRD_PARTY.md`)
- [x] CI: build (uyarılar hata sayılır) + bekleyen model değişikliği kontrolü

## Aşama 2 — Mağaza arayüzü (Eticaret) yeniden tasarım
- [x] `docs/DESIGN.md` token'larıyla yeni layout: header (arama, sepet sayacı, kullanıcı menüsü), footer
- [x] Ana sayfa, kategori/ürün listesi (filtre + sıralama + **sayfalama**), ürün detayı (galeri, yorumlar, puan)
- [x] Sepet, ödeme (checkout) ve sipariş detay sayfaları; adım göstergesi
- [x] Profil: siparişlerim, ürünlerim (satıcı), satıcı başvurusu
- [x] Mobil uyum (360px'te kusursuz)

## Aşama 3 — Admin paneli yeniden tasarım
- [ ] SB Admin 2 görünümü kaldırılır; kendi sade admin layout'u
- [ ] Dashboard: özet kartlar (sipariş, ciro, yeni kullanıcı, bekleyen yorum) + basit grafik
- [ ] Ürün, kategori, kullanıcı, yorum listeleri: arama, filtre, sayfalama, toplu işlem
- [ ] Satıcı başvurusu onay akışı, yorum onay akışı

## Aşama 4 — Özellik tamamlama
- [ ] Blog: seed verisi ve TODO'ların kaldırılması, kategori/etiket filtreleri
- [ ] İndirim kodu (DiscountEntity) checkout'ta çalışır
- [ ] Şifremi unuttum / yenile akışı uçtan uca çalışır (e-posta ayarı env'den)
- [ ] Sipariş durumları (Hazırlanıyor → Kargoda → Teslim edildi) admin'den güncellenir

## Aşama 5 — Test, yayın, README
- [ ] Test projesi: servis katmanı için xUnit testleri (sepet, sipariş, indirim)
- [ ] GitHub Actions: build + test
- [ ] `docker-compose` (Eticaret + Admin + FileApi + SQL Server)
- [ ] Canlı demo + demo hesapları (müşteri, satıcı, admin)
- [ ] Ekran görüntüleri, GIF, README (TR + EN) ve mimari şeması
