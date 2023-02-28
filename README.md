# Exempel-webb med .NET MVC + API + Vue-frontend
Ett litet exempel, där en MVC-webbplats med Identity används som admin-gränssnitt, och en API används för att visa data.
Vue används som exempel för att konsumera API'et.

## Delar
MVC-webbplats med Identity, och API-controllers - mappen "f1-backend".
Startas med: dotnet run
Registrera konto -> logga in -> administrera

### API endpoints
**DriversApiController:**
* GET /api/drivers
* GET /api/drivers/{id}
* GET /api/drivers/team/{id}

**TeamsApiController:**
* GET /api/teams
* GET /api/teams/{id} 

## Vue-applikation
Öppna direkt via f1-frontend/index.html, eller kör via VSC Live reload.

## NuGet-paket
Paketet **SixLabors.ImageSharp** används för att skala om bilder.
Görs endast för förarbilder, gör thumbnails och sparar om i WebP-format (DriverController).
Läs mer: https://sixlabors.com/products/imagesharp/

## Av
Mattias Dahlgren, 2023