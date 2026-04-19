---
name: configure-cicd-pipeline
description: GitHub Actions CI/CD pipeline yapılandırma. .NET backend build, test ve publish; Vue.js frontend build; Docker image build ve push; SonarQube kod kalite analizi ve deployment adımlarını içeren workflow dosyaları oluşturur veya günceller.
---

# Skill: CI/CD Pipeline Yapılandırma

## Ne Zaman Kullanılır
- Yeni bir GitHub Actions workflow oluşturulacaksa
- Mevcut pipeline'a adım eklenecekse (SonarQube, Docker, vb.)
- Build veya deploy adımlarında sorun yaşanıyorsa
- Docker Compose veya Dockerfile güncellenecekse

## Repo Yapısı

```
.github/
└── workflows/
    ├── backend-ci.yml     # .NET build + test + SonarQube
    └── frontend-ci.yml    # Vue build + test
docker-compose.yml         # Yerel geliştirme ortamı
src/
├── backend/               # .NET 9 solution
└── frontend/              # Vue 3 + Vite
```

## Backend CI Workflow

```yaml
# .github/workflows/backend-ci.yml
name: Backend CI

on:
  push:
    branches: [main, develop]
    paths:
      - 'src/backend/**'
      - 'tests/**'
      - '.github/workflows/backend-ci.yml'
  pull_request:
    branches: [main, develop]

env:
  DOTNET_VERSION: '9.0.x'

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0  # SonarQube için tam geçmiş

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore dependencies
        run: dotnet restore src/backend/VehicleInventory.slnx

      - name: Build
        run: dotnet build src/backend/VehicleInventory.slnx --configuration Release --no-restore

      - name: Run Tests
        run: |
          dotnet test tests/ \
            --configuration Release \
            --no-build \
            --collect:"XPlat Code Coverage" \
            --results-directory ./coverage

      - name: SonarQube Analysis
        uses: SonarSource/sonarqube-scan-action@master
        env:
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
          SONAR_HOST_URL: ${{ secrets.SONAR_HOST_URL }}
```

## Frontend CI Workflow

```yaml
# .github/workflows/frontend-ci.yml
name: Frontend CI

on:
  push:
    branches: [main, develop]
    paths:
      - 'src/frontend/**'
      - '.github/workflows/frontend-ci.yml'
  pull_request:
    branches: [main, develop]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'

      - name: Install Yarn
        run: npm install -g yarn

      - name: Install dependencies
        working-directory: src/frontend
        run: yarn install --frozen-lockfile

      - name: Type check
        working-directory: src/frontend
        run: yarn vue-tsc --noEmit

      - name: Build
        working-directory: src/frontend
        run: yarn build
```

## Docker Compose (Yerel Geliştirme)

```yaml
# docker-compose.yml içine eklenecek servis örneği
services:
  api:
    build:
      context: ./src/backend
      dockerfile: VehicleInventory.API/Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=vehicleinventory;Username=postgres;Password=postgres
    depends_on:
      - postgres

  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: vehicleinventory
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## .NET Dockerfile

```dockerfile
# src/backend/VehicleInventory.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["VehicleInventory.API/VehicleInventory.API.csproj", "VehicleInventory.API/"]
COPY ["VehicleInventory.Application/VehicleInventory.Application.csproj", "VehicleInventory.Application/"]
COPY ["VehicleInventory.Domain/VehicleInventory.Domain.csproj", "VehicleInventory.Domain/"]
COPY ["VehicleInventory.Infrastructure/VehicleInventory.Infrastructure.csproj", "VehicleInventory.Infrastructure/"]
RUN dotnet restore "VehicleInventory.API/VehicleInventory.API.csproj"
COPY . .
RUN dotnet build "VehicleInventory.API/VehicleInventory.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VehicleInventory.API/VehicleInventory.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VehicleInventory.API.dll"]
```

## Sık Karşılaşılan Sorunlar

| Sorun | Çözüm |
|-------|-------|
| `dotnet restore` başarısız | `nuget.config` veya private feed credential'ları kontrol et |
| SonarQube token hatası | `SONAR_TOKEN` ve `SONAR_HOST_URL` secret'larını ekle |
| Docker build cache sorunları | `--no-cache` flag ekle veya `COPY . .` öncesi restore optimize et |
| Frontend `yarn install` hatası | `--frozen-lockfile` flag'i kaldır veya `yarn.lock` dosyasını commit'le |
| Test coverage yüklenmedi | `coverlet.collector` NuGet paketi test projesinde var mı? |

## Kontrol Listesi
- [ ] Workflow dosyası `.github/workflows/` altında
- [ ] `on.push.paths` filtrelemesi doğru ayarlandı
- [ ] `dotnet restore` solution dosyasını hedefliyor
- [ ] Test komutu coverage topluyorsa `coverlet.collector` paketi mevcut
- [ ] SonarQube için `fetch-depth: 0` ayarlandı
- [ ] Secrets (`SONAR_TOKEN`, `SONAR_HOST_URL`) repository settings'te tanımlı
- [ ] Frontend workflow'da `--frozen-lockfile` kullanılıyor
