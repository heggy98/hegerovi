using QRCoder;

if (args.Length < 2)
{
    Console.WriteLine("Použití: QrGenerator <cesta-k-csv> <výstupní-složka> [base-url]");
    Console.WriteLine("CSV formát: Token,GuestName (bez hlavičky)");
    return 1;
}

var csvPath = args[0];
var outputDir = args[1];
var baseUrl = args.Length > 2 ? args[2] : "https://svatba.cz/rsvp";

if (!File.Exists(csvPath))
{
    Console.WriteLine($"Soubor {csvPath} neexistuje.");
    return 1;
}

Directory.CreateDirectory(outputDir);

using var generator = new QRCodeGenerator();
var lines = File.ReadAllLines(csvPath);
var count = 0;

foreach (var line in lines)
{
    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    var parts = line.Split(',');
    var token = parts[0].Trim();
    var guestName = parts.Length > 1 ? parts[1].Trim() : token;

    var url = $"{baseUrl.TrimEnd('/')}/{token}";
    using var qrData = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
    using var qrCode = new PngByteQRCode(qrData);
    var pngBytes = qrCode.GetGraphic(20);

    var safeFileName = string.Join("_", guestName.Split(Path.GetInvalidFileNameChars()));
    var outputPath = Path.Combine(outputDir, $"{safeFileName}_{token}.png");
    File.WriteAllBytes(outputPath, pngBytes);

    Console.WriteLine($"Vygenerováno: {outputPath} -> {url}");
    count++;
}

Console.WriteLine($"Hotovo. Vygenerováno {count} QR kódů do {outputDir}.");
return 0;
