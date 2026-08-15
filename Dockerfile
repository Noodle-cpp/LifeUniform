FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LifeUniform.Domain/LifeUniform.Domain.csproj LifeUniform.Domain/
COPY LifeUniform.Application/LifeUniform.Application.csproj LifeUniform.Application/
COPY LifeUniform.Infrastructure/LifeUniform.Infrastructure.csproj LifeUniform.Infrastructure/
COPY LifeUniform.Web/LifeUniform.Web.csproj LifeUniform.Web/
RUN dotnet restore LifeUniform.Web/LifeUniform.Web.csproj

COPY LifeUniform.Domain/ LifeUniform.Domain/
COPY LifeUniform.Application/ LifeUniform.Application/
COPY LifeUniform.Infrastructure/ LifeUniform.Infrastructure/
COPY LifeUniform.Web/ LifeUniform.Web/
RUN dotnet publish LifeUniform.Web/LifeUniform.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/uploads/products \
    && chown -R app:app /app

USER app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "LifeUniform.Web.dll"]
