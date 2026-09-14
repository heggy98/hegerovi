# Svatební web – shrnutí zadání

Datum svatby: 12.6.
Cíl: reprezentativní svatební web + RSVP systém s QR kódy na papírových pozvánkách.

## Moje dovednosti
C#, Angular, klasický JS/TS, HTML, CSS.

## Architektura
- **Backend:** Azure Functions (C#), Cosmos DB free tier jako databáze
- **Frontend:** Angular, hostováno na Azure Static Web Apps (free tier)
- **Doména:** vlastní .cz doména, napojená na Static Web Apps (SSL zdarma)
- **Deploy:** GitHub repo napojené na Static Web Apps přes GitHub Actions (auto-deploy při push)

## Datový model (návrh)
- `Guest`: jméno, e-mail, telefon, skupina, povolen plus-one (ano/ne), poznámka
- `Invitation`: vazba na Guest, unikátní token (GUID/nanoid), stav (pending/confirmed/declined), počet osob, menu preference, alergie

## RSVP flow
- Každý host má unikátní odkaz `svatba.cz/rsvp/{token}`
- QR kódy se generují PŘEDEM, offline, malým C# konzolovým skriptem (knihovna QRCoder) – ne za běhu appky
- QR/URL se vytiskne na papírové pozvánky
- Host naskenuje QR → otevře RSVP formulář → vyplní účast, počet osob, menu, alergie
- Endpointy: `GET/POST /api/rsvp/{token}` (veřejné), CRUD endpointy pro hosty (chráněné, jen pro admina)

## Obsah webu
- Veřejná část: příběh, místo konání, harmonogram, mapa, RSVP formulář (přes token v URL)
- Admin část (chráněná route ve stejné Angular appce): tabulka hostů, import z CSV, přehled RSVP odpovědí, export pro cateringa
- Po svatbě: galerie fotek (přes Cloudinary free tier nebo odkaz na Google Photos/iCloud album – neřešit vlastní storage)

## Náklady
Cíl ~250 Kč/rok (jen doména), zbytek na free tierech Azure.

## Fáze realizace
0. Doména, Azure účet, GitHub repo
1. Backend jádro (Functions + Cosmos DB + endpointy)
2. QR generátor (lokální C# skript)
3. Veřejný web (Angular)
4. Admin rozhraní
5. Nasazení + test na doméně
6. Po svatbě: galerie fotek
