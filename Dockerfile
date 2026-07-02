FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Instalar Tesseract OCR + paquetes de idioma (ingles y espanol)
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        tesseract-ocr \
        tesseract-ocr-eng \
        tesseract-ocr-spa \
        libleptonica-dev && \
    LEPT_LIB=$(find / -name "liblept.so.*" 2>/dev/null | head -n1) && \
    if [ -n "$LEPT_LIB" ]; then \
        ln -sf "$LEPT_LIB" /usr/lib/x86_64-linux-gnu/libleptonica-1.82.0.so; \
    fi && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "SafeNet.Web/SafeNet.Web.csproj"
RUN dotnet publish "SafeNet.Web/SafeNet.Web.csproj" -c Release -o /app/out

FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "SafeNet.Web.dll"]