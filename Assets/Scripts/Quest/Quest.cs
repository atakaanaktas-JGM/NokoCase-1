using UnityEngine;

public enum QuestType
{
    FIGHT,
    HUNT,
    GATHER,
    TALK,
    EXPLORE
}

[CreateAssetMenu(menuName = "RPG/Quest")]
public class Quest : ScriptableObject
{
    public string uID;
    public string title;
    public string description;
    public int experience;
    public int gold;

    public int amount;
    public string[] targets;

    public string talkTo;
    public Vector3 explore;

    public string questGiver;
    public QuestType type;
}
