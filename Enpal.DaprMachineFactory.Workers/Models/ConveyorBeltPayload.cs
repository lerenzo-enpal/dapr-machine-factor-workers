namespace Enpal.DaprMachineFactory.Workers.Models;

public record ConveyorBeltPayload(string SystemIdentifier, IList<Asset> Assets);

public enum AssetClass
{
    Battery,
    Wallbox,
    Heatpump,
    Inverter
}

public record Asset(AssetClass AssetClass, string EuropeanArticleNumber);
