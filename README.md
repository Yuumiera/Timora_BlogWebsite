# Timora Blog Website

ASP.NET Core 9.0 tabanlı blog uygulaması.

## Gereksinimler

- .NET 9 SDK
- Entity Framework Core CLI (opsiyonel): `dotnet tool install --global dotnet-ef`

## Kurulum ve Çalıştırma

1. Proje dizinine gidin:
   ```powershell
   cd src/Timora.Blog
   ```

2. Bağımlılıkları yükleyin:
   ```powershell
   dotnet restore
   ```

3. Veritabanını oluşturun:
   ```powershell
   dotnet ef database update
   ```

4. Uygulamayı çalıştırın:
   ```powershell
   dotnet run
   ```

5. Tarayıcıda açın:
   - HTTPS: `https://localhost:5001`
   - HTTP: `http://localhost:5000`

## Geliştirme

Sıcak yeniden yükleme ile çalıştırmak için:
```powershell
dotnet watch run
```

