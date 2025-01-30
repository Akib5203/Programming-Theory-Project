public class DistanceAchievement
{
    public bool Meters10 { get; set; }
    public bool Meters20 { get; set; }
    public bool Meters30 { get; set; }
    public bool Meters40 { get; set; }
    public bool Meters50 { get; set; }
    public bool Meters60 { get; set; }
    public bool Meters70 { get; set; }
    public bool Meters80 { get; set; }
    public bool Meters90 { get; set; }
    public bool Meters100 { get; set; }
    public bool Meters500 { get; set; }
    public bool Meters1000 { get; set; }

    // For Debug Purposes
    public override string ToString() => $"10 Meters: {Meters10}, 20 Meters: {Meters20}, 30 Meters: {Meters30}, 40 Meters: {Meters40}, 50 Meters: {Meters50}, 60 Meters: {Meters60}, 70 Meters: {Meters70}, 80 Meters: {Meters80}, 90 Meters: {Meters90}, 100 Meters: {Meters100}, 500 Meters: {Meters500}, 1000 Meters: {Meters1000}";
}
