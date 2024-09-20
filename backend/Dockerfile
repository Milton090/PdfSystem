FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["/PdfSystem/PdfSystem.csproj", "PdfSystem/"]
COPY ["PdfSystemTest/PdfSystemTest.csproj", "PdfSystemTest/"]

RUN dotnet restore "PdfSystem/PdfSystem.csproj"
RUN dotnet restore "PdfSystemTest/PdfSystemTest.csproj"

COPY . .

WORKDIR "/src/PdfSystem"
RUN dotnet build "PdfSystem.csproj" -c Release -o /app/build

WORKDIR "/src/PdfSystemTest"
RUN dotnet test "PdfSystemTest.csproj" -c Release -o /app/test


WORKDIR "/src/PdfSystem"
FROM build AS publish
RUN dotnet publish "PdfSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY PdfSystem/Certificate/certificate.pfx /app/Certificate/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PdfSystem.dll"]