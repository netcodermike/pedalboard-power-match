using System.Text.Json;

try
{
    if (args.Length != 1) throw new ArgumentException("Pass one JSON input file");
    using var data = JsonDocument.Parse(File.ReadAllText(args[0]));
    Console.WriteLine(JsonSerializer.Serialize(Analyze(data.RootElement)));
    return 0;
}
catch (Exception error) when (error is IOException or JsonException or FormatException
                               or ArgumentException or InvalidOperationException
                               or KeyNotFoundException or OverflowException)
{
    Console.Error.WriteLine(error.Message);
    return 2;
}

static List<(string Name, decimal Voltage, int Current)> ReadPorts(JsonElement source)
{
    var rows = new List<(string Name, decimal Voltage, int Current)>();
    foreach (var item in source.EnumerateArray())
    {
        if (item.GetArrayLength() != 3) throw new ArgumentException("Each entry requires name, voltage, and current");
        string name = item[0].GetString() ?? "";
        decimal voltage = item[1].GetDecimal();
        int current = item[2].GetInt32();
        if (String.IsNullOrWhiteSpace(name) || voltage <= 0 || current <= 0 || rows.Any(row => row.Name == name))
            throw new ArgumentException("Invalid or duplicate entry");
        rows.Add((name,voltage,current));
    }
    return rows;
}

static object Analyze(JsonElement input)
{
    var pedals = ReadPorts(input.GetProperty("pedals"));
    var ports = ReadPorts(input.GetProperty("outputs"));
    var available = ports.OrderBy(port => port.Current).ThenBy(port => port.Name, StringComparer.Ordinal).ToList();
    var matches = new Dictionary<string,string>();
    var unassigned = new List<string>();
    foreach (var pedal in pedals.OrderByDescending(pedal => pedal.Current))
    {
        int index = available.FindIndex(port => port.Voltage == pedal.Voltage && port.Current >= pedal.Current);
        if (index < 0) { unassigned.Add(pedal.Name); continue; }
        matches.Add(pedal.Name, available[index].Name);
        available.RemoveAt(index);
    }
    return new { matches, unassigned };
}
