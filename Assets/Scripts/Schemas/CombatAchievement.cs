public class CombatAchievement
{
    public bool Punches10 { get; set; }
    public bool Punches20 { get; set; }
    public bool Punches50 { get; set; }
    public bool Punches100 { get; set; }
    public bool Kicks10 { get; set; }
    public bool Kicks20 { get; set; }
    public bool Kicks50 { get; set; }
    public bool Kicks100 { get; set; }
    public bool Hits20 { get; set; }
    public bool Hits40 { get; set; }
    public bool Hits100 { get; set; }
    public bool Hits500 { get; set; }

    // For Debug Purposes
    public override string ToString() => $"10 Punches: {Punches10}, 10 Kicks: {Kicks10}, 20 Punches: {Punches20}, 20 Kicks: {Kicks20}, 50 Punches: {Punches50}, 50 Kicks: {Kicks50}, 100 Punches: {Punches100}, 100 Kicks: {Kicks100}, 20 Total Hits: {Hits20}, 40 Total Hits: {Hits40}, 100 Total Hits: {Hits100}, 500 Total Hits: {Hits500}";
}
