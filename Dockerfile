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
    mkdir -p /app/x64 && \
    LEPT_LIB=$(find / -name "liblept.so.*" 2>/dev/null | head -n1) && \
    if [ -n "$LEPT_LIB" ]; then \
        ln -sf "$LEPT_LIB" /app/x64/libleptonica-1.82.0.so; \
    fi && \
    TESS_LIB=$(find / -name "libtesseract.so.*" 2>/dev/null | head -n1) && \
    if [ -n "$TESS_LIB" ]; then \
        ln -sf "$TESS_LIB" /app/x64/libtesseract50.so; \
    fi && \
    DL_LIB=$(find / -name "libdl.so.2" 2>/dev/null | head -n1) && \
    if [ -n "$DL_LIB" ]; then \
        ln -sf "$DL_LIB" /app/libdl.so && \
        ln -sf "$DL_LIB" /app/x64/libdl.so; \
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