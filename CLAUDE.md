# hegerovi — svatební web

Osobní projekt (svatební web + RSVP systém), nesouvisí s Astratexem ani žádnou firemní prací.

## Nasazení

- Testovací adresa (Azure Static Web Apps, uživatelův soukromý účet): https://ashy-meadow-047072510.5.azurestaticapps.net/
- Auto-deploy z `main` přes GitHub Actions workflow, který si Azure sám přidal do repa při založení Static Web App.
- Cosmos DB zatím NENÍ založená — backend běží na `InMemoryDataStore`, takže RSVP data se ztratí při restartu API. Až bude Cosmos DB hotová, přepnout podle sekce Architektura níže.

## Tvrdá pravidla

- Nikdy nepoužívat žádné zdroje od Astratexu — žádné firemní NuGet feedy, žádný Astratex Azure
  tenant/účet. `NuGet.config` v rootu je záměrně omezený jen na `nuget.org`.
- Azure samotné vadit nemá — projekt se nasazuje na Azure pod uživatelovým SOUKROMÝM účtem
  (ne pracovním Astratex tenantem). Přihlašování do Azure a zakládání resources dělá uživatel
  sám v portálu (vyžaduje jeho vlastní přihlášení), Claude nespouští `az login` ani neinstaluje
  Azure CLI bez výslovného souhlasu.
- Nedívat se do ostatních projektů v `C:\source` (jsou to nesouvisející firemní repa).

## Architektura

- `backend/` — Azure Functions, .NET 8 isolated worker. Datový přístup je za rozhraním
  `IDataStore` (`Services/IDataStore.cs`). Výchozí implementace je `InMemoryDataStore`
  (`DataProvider=InMemory` v `local.settings.json`) — nic se nemusí instalovat.
  `CosmosDataStore` je hotová vedle a jde přepnout nastavením `DataProvider=Cosmos` +
  `CosmosDbConnectionString`, až bude k dispozici Cosmos DB Emulator nebo účet.
- `frontend/` — Angular, mobile-first (základní styly v `styles.scss` cílí na mobil,
  rozšíření pro širší displeje jsou přes `@media (min-width: ...)`).
- `tools/QrGenerator/` — C# konzolová appka (QRCoder), generuje QR kódy z CSV (token,jméno)
  pro papírové pozvánky.
- Admin endpointy (`guests`, `invitations`) jsou chráněné jednoduchou hlavičkou `x-admin-key`
  proti `AdminApiKey` z konfigurace — žádný plnohodnotný auth systém, stačí to na malý seznam hostů.

## Git

- Push na `origin/main` tohoto repa je povolený bez ptaní. Force push, jiné branche nebo
  destruktivní operace pořád vyžadují potvrzení.
