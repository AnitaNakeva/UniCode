FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app
 
COPY main.cs .

# Създай нов проект
RUN dotnet new console --force

# Замести Program.cs с твоя main.cs
RUN mv main.cs Program.cs

# Компилирай
RUN dotnet build -c Release -o /app/build

# Стартирай
CMD ["dotnet", "/app/build/app.dll"]