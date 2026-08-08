# Evidence rezervací místností .NET

## Použitý stack a verze

| Technologie | Verze | Použití |
| .NET SDK | 10 | sestavení a spuštění projektu |
| .NET / ASP.NET Core MVC | 10.0 | backend |
| Entity Framework Core | 10.0 | práce s databází |
| Bootstrap | 5.3.3 | základní rozložení a komponenty |
| JavaScript, HTML a CSS | bez frameworku | formuláře, validace a vlastní vzhled |

## Požadavky

- nainstalované .NET SDK 10.0 nebo novější kompatibilní verze,
- libovolný webový prohlížeč 

Nainstalovanou verzi .NET lze ověřit příkazem:

~~~powershell
dotnet --version
~~~

## Jak aplikaci spustit

1. Naklonujte repozitář a přejděte do jeho kořenové složky.
2. Obnovte NuGet balíčky:

~~~powershell
dotnet restore .\EvidenceRezervaceMistnosti.slnx
~~~

3. Sestavte projekt:

~~~powershell
dotnet build .\EvidenceRezervaceMistnosti.slnx
~~~

4. Spusťte aplikaci:

~~~powershell
dotnet run --project .\EvidenceRezervaceMistnosti\EvidenceRezervaceMistnosti.csproj --launch-profile https
~~~

5. V prohlížeči otevřete:

~~~text
https://localhost:7082
~~~

Aplikace je dostupná také na **http://localhost:5140**. 
Pokud počítač nedůvěřuje lokálnímu HTTPS certifikátu, lze ho připravit příkazem:

~~~powershell
dotnet dev-certs https --trust
~~~

### Swagger a OpenAPI

Při spuštění je interaktivní dokumentace dostupná na:

~~~text
https://localhost:7082/swagger
~~~

### Jedna MVC aplikace

Backend, Razor views i API endpointy jsem nechal v jednom projektu. 
Pro rozsah této aplikace je to jednodušší na spuštění i orientaci než rozdělený frontend a backend.

### SQLite v paměti

Místo databázového serveru jsem použil SQLite v paměti. 
Aplikaci se dá pustit bez další konfigurace.

### Půlhodinové časové sloty

Časy rezervací se vybírají po 30 minutách. 
Obsazené možnosti frontend zakáže a backend ještě jednou kontroluje skutečný překryv. 
Rezervace na sebe mohou přímo navazovat, například **09:00–10:00** a **10:00–10:30**.

### Validace na frontendu i backendu

Jednoduchou uživatelskou validaci jsem přidal do JavaScriptu, aby uživatel dostal chybu ihned.
Pravidla zůstali také na backendu, protože frontend lze obejít. 

### Lokalizace

Texty jsou připravené pro češtinu, angličtinu a němčinu. Výchozí jazyk je čeština.