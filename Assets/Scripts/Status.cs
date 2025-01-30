// In this CS file we just created small usable enums and Serializable class

public enum Status
{
    Completed, NotCompleted
}

public enum AchievementType
{
    Runner, Fighter
}

public enum CombatType
{
    None, Punch, Kick, Total
}

[System.Serializable]
public class AchievementKeyValuePair
{
    public CombatType CombatType;
    public int AchievementGoal;
    public string PropertyName;
}