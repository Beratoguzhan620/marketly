# Configuration

`appsettings.json` only contains safe defaults. Real secrets are supplied through
[User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) in development
or environment variables in production.

| Setting | Apps | Default | Notes |
|---|---|---|---|
| `ConnectionStrings:SqlServer` | Eticaret, Admin | LocalDB `MarketlyDb` | Both apps share one database. Migrations run automatically at start-up. |
| `FileApi:BaseAddress` | Eticaret, Admin | `http://localhost:5031/` (`https://localhost:7018/` in Development) | URL of `App.FileApi`. |
| `FileApi:ApiKey` | Eticaret, Admin | `local-dev-key` in Development | Must match `Authentication:ApiKey` of the File API. |
| `Authentication:ApiKey` | FileApi | `local-dev-key` in Development | Required for upload/delete. |
| `Email:Host`, `Email:Username`, `Email:Password`, `Email:FromAddress` | Eticaret, Admin | empty | When `Host` is empty no e-mail is sent; in Development the reset link is shown on the page instead. |
| `Email:StorefrontUrl` | Eticaret, Admin | empty | Base URL for links inside e-mails. Empty means the scheme and host of the incoming request. Set it when the app runs behind a proxy. |

## Examples

```bash
# Development (run inside the project folder)
dotnet user-secrets set "ConnectionStrings:SqlServer" "Server=.;Database=MarketlyDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Email:Host" "smtp.gmail.com"
dotnet user-secrets set "Email:Username" "you@gmail.com"
dotnet user-secrets set "Email:Password" "<app password>"
dotnet user-secrets set "Email:FromAddress" "you@gmail.com"
```

```bash
# Production (environment variables use double underscores)
ConnectionStrings__SqlServer="Server=db;Database=MarketlyDb;User Id=sa;Password=...;TrustServerCertificate=True"
FileApi__ApiKey="..."
Authentication__ApiKey="..."
```

## Demo accounts

All seeded accounts use the password `Demo123!`.

| Role | E-mail |
|---|---|
| Admin | `admin@example.com` |
| Seller | `seller@example.com`, `ece.demir@example.com`, `can.ozturk@example.com` |
| Buyer | `buyer@example.com`, `burak.sahin@example.com`, `elif.arslan@example.com`, `deniz.celik@example.com` |
