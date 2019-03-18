FROM microsoft/dotnet:2.2-runtime-stretch-slim AS base
WORKDIR /app


FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY ["TestSendCore/TestSendCore.csproj", "TestSendCore/"]
COPY ["Decos.Diagnostics.Trace/Decos.Diagnostics.Trace.csproj", "Decos.Diagnostics.Trace/"]
COPY ["Decos.Diagnostics/Decos.Diagnostics.csproj", "Decos.Diagnostics/"]
RUN dotnet restore "TestSendCore/TestSendCore.csproj"
COPY . .
WORKDIR "/src/TestSendCore"
RUN dotnet build "TestSendCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TestSendCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TestSendCore.dll"]