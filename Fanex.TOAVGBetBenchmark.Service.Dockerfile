FROM hub.nexdev.net/base/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder
WORKDIR /src

COPY ./Fanex.TOAVGBetBenchmark.Service/Fanex.TOAVGBetBenchmark.Business ./Fanex.TOAVGBetBenchmark.Business
COPY ./Fanex.TOAVGBetBenchmark.Service/Fanex.TOAVGBetBenchmark.Service/Fanex.TOAVGBetBenchmark.Service.csproj ./Fanex.TOAVGBetBenchmark.Service/Fanex.TOAVGBetBenchmark.Service.csproj

RUN ls /src -l
WORKDIR /src/Fanex.TOAVGBetBenchmark.Service
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

COPY ./Fanex.TOAVGBetBenchmark.Service/Fanex.TOAVGBetBenchmark.Service .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/rebuild-baseimage/dotnetcore/aspnet:5.0-nap-mail-core

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

ENV TZ="America/La_Paz" 

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.TOAVGBetBenchmark.Service.dll"]
