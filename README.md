# Timora Blog Website

Timora Blog, ilham verici içerikleri yayına almak, yazar profillerini yönetmek ve çok dilli bir topluluk deneyimi sunmak için geliştirilen tam özellikli bir ASP.NET Core 9.0 uygulamasıdır. Proje, içerik üreticilerine kategori bazlı yayıncılık, zengin profil alanları, tema/erişilebilirlik araçları ve responsive tasarımla desteklenen modern bir deneyim sağlar.

## Genel Bakış

- **.NET 9 + ASP.NET Core MVC**: Razor tabanlı UI, güçlü yönlendirme ve Model-View-Controller ayrımı.
- **Entity Framework Core + SQLite**: Kimlik, gönderi ve profil verileri için tek dosyalı `App_Data/blog.db`.
- **ASP.NET Identity**: E-posta tabanlı kayıt/giriş, parola politikaları ve profil senkronizasyonu.
- **Çok dilli arayüz**: 15 dil, RTL desteği, çerez tabanlı kültür seçimi ve slug bazlı çeviri.
- **HCI/erişilebilirlik geliştirmeleri**: Tema tuşları, toast bildirimi, onay modalları, skip-link, duyurular.
- **Görsel yönetimi**: Kapak ve profil görselleri `wwwroot/uploads` altında fiziksel olarak saklanır.

## Mimari & Teknoloji Yığını

| Katman | Teknolojiler |
| --- | --- |
| Sunum | ASP.NET Core MVC, Razor Views, Bootstrap 5, özel `site.css`, jQuery |
| Uygulama | Controllers + ViewComponents + ViewModels, `IWebHostEnvironment` ile medya yönetimi |
| Veri | Entity Framework Core 9, SQLite, Fluent konfigurasyon ve seed’ler |
| Kimlik | ASP.NET Core Identity (`AddDefaultIdentity<IdentityUser>`) + cookie yapılandırması |
| Yerelleştirme | `RequestLocalizationOptions`, `LanguageStrings` statik sözlüğü, cookie provider |

## Ana Modüller

- **Blog Yönetimi (`BlogController`)**
  - Yalnızca yayınlanmış yazıları listeler; kategori slugu ile filtreler.
  - Slug üretimi ve eşsizliği, tarih damgası, kategori/breadcrumb çevirileri.
  - Kapak görseli doğrulaması ve `wwwroot/uploads` klasörüne güvenli yükleme.
  - Yazarı doğrulama (sadece yazan kullanıcı düzenleyebilir/silebilir).

- **Kimlik & Hesap (`AccountController`)**
  - E-posta ve parola ile kayıt/giriş; başarısız girişleri kilitleme opsiyonu.
  - Kayıt sırasında eş zamanlı `UserProfile` oluşturma ve isteğe bağlı yaş → doğum tarihi dönüşümü.

- **Profil Yönetimi (`ProfileController`)**
  - Her kullanıcı için biyografi, iletişim, hobiler ve görsel alanları.
  - Profil kartında hızlı yazı oluşturma/düzenleme/ silme aksiyonları.
  - Profil fotoğrafları `wwwroot/uploads/profiles` dizinine yazılır.

- **Yerelleştirme & Tema**
  - `LanguageController` çerez üzerinden kültürü kalıcı kılar.
  - `LanguageStrings` 15 kültüre kadar statik çeviri sağlar (TR, EN, DE, FR, ES, AR, RU, IT, PT-BR, JA, ZH-CN, KO, NL, PL, SV).
  - `wwwroot/js/site.js` üç tema (light/dark/reading), toast, onay diyaloğu, form geliştirmeleri ve klavye kısayolları sunar.
  - RTL diller için `<html dir="rtl">` otomatik ayarlanır.

## Proje Yapısı

```
Timora_BlogWebsite/
├─ Timora_BlogWebsite.sln
└─ src/Timora.Blog/
   ├─ Controllers/           # Blog, Account, Home, Profile, Language
   ├─ Data/                  # AppDbContext + EF konfigürasyonu
   ├─ Models/
   │  ├─ Entities (Post, Category, UserProfile)
+  │  └─ ViewModels (Register, Login, PostCreate, ProfileEdit, Breadcrumb)
   ├─ Migrations/            # EF Core migration geçmişi
   ├─ Views/                 # Razor sayfaları ve layout’lar
   ├─ ViewComponents/        # Kategori menüsü
   ├─ wwwroot/               # Statik varlıklar, tema CSS/JS, upload klasörleri
   ├─ appsettings*.json      # Ortam bazlı konfigürasyonlar
   └─ Timora.Blog.csproj
```

## Veritabanı Modeli

| Tablo | Açıklama |
| --- | --- |
| `Posts` | Başlık, slug, içerik, yayın durumu, kapak görseli, kategori & yazar ilişkileri |
| `Categories` | 11 adet seed'li kategori, slug indeksleri |
| `UserProfiles` | Identity kullanıcısına bağlı genişletilmiş profil alanları |
| `AspNet*` | Identity çerçevesinin standart tabloları |

- `AppDbContext` ilişkileri `OnModelCreating` içinde tanımlar, `DeleteBehavior.SetNull` ile yazar/kategori silinmelerinde içeriğin korunmasını sağlar.
- Yeni migration oluşturmak için `dotnet ef migrations add <Isim>` komutu kullanılabilir.

## Yerelleştirme

- Varsayılan kültür `tr-TR`; desteklenen kültürler `LanguageStrings.GetSupportedCultures()` ile listelenir.
- Header’daki dil açılır menüsü `LanguageController.SetLanguage` aksiyonunu POST ederek kültür çerezini günceller.
- Breadcrumb, kategori etiketleri ve sabit metinler slug bazlı sözlükten çevrilir.

## Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- (Opsiyonel) EF Core CLI: `dotnet tool install --global dotnet-ef`
- SQLite 3 dosya sürücüsü; Windows’ta ek kurulum gerektirmez.

## Kurulum Adımları

1. **Projeyi alın**
   ```powershell
   git clone <repo-url> Timora_BlogWebsite
   cd Timora_BlogWebsite/src/Timora.Blog
   ```
2. **Bağımlılıkları geri yükleyin**
   ```powershell
   dotnet restore
   ```
3. **Veritabanını oluşturun/güncelleyin**
   ```powershell
   dotnet ef database update
   ```
   - Komut `App_Data/blog.db` dosyasını oluşturur ve seed verileri yükler.
4. **Uygulamayı başlatın**
   ```powershell
   dotnet run
   ```
   - Varsayılan olarak `https://localhost:5001` ve `http://localhost:5000` dinlenir.

## Geliştirme Akışı

- **Hızlı iterasyon**: `dotnet watch run` ile sıcak yeniden yükleme.
- **Kod kalitesi**: `dotnet build` / `dotnet format` (isteğe bağlı).
- **Migration yönetimi**:
  ```powershell
  dotnet ef migrations add AddNewFeature
  dotnet ef database update
  ```
- **Çevresel konfigürasyon**: `appsettings.Development.json` dosyasında sadece geliştirmeye özel ayarlar barındırılır; yeni anahtarlar ekleyebilirsiniz.

## Konfigürasyon Notları

- `appsettings.json` içindeki `ConnectionStrings:DefaultConnection` SQLite yolunu belirler. Farklı bir veri yolu istiyorsanız göreli/absolute path verebilirsiniz.
- Production ortamında `ASPNETCORE_ENVIRONMENT=Production` ve `AllowedHosts` kısıtlamalarını güncelleyin.
- Statik dosya yüklemeleri için **uzun süreli saklama** gerekiyorsa CDN veya blob storage’a taşıyacak servis katmanı eklenebilir.

## Statik Varlıklar ve Yüklemeler

- Kapak fotoğrafları: `wwwroot/uploads/<guid>.<ext>`
- Profil fotoğrafları: `wwwroot/uploads/profiles/<guid>.<ext>`
- Yüklenen dosyalar Git deposuna dahil değildir; gerektiğinde `wwwroot/uploads` klasörü oluşturulup yazma yetkisi verilmelidir.

## UX / HCI Özellikleri

- Skip-to-content bağlantısı, form validasyonu, submit sırasında loading göstergeleri ve ekran okuyucu anonsları.
- Üç tema modu, toast bildirimleri ve çıkış onayı `site.js` üzerinden merkezi olarak yönetilir.
- Tüm renk paleti WCAG AAA kontrastı hedefler, tipografi clamp fonksiyonlarıyla tüm ekranlarda okunabilirlik sağlar.

## Yol Haritası Önerileri

- Dinamik arama ve etiketleme sistemi,
- İçerik planlama için taslak/publish akışı,
- API uçları veya Blazor tabanlı istemci,
- Medya yüklemeleri için boyut sınırlaması ve optimize servisleri,
- Otomatik test paketi (unit/integration) ve GitHub Actions pipeline’ı.

---

**Destek**: Proje hakkında sorularınız veya katkı talepleriniz için Issues sekmesini kullanabilir ya da doğrudan ekip ile iletişime geçebilirsiniz. Mutlu kodlamalar! 🎉