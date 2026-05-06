using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    ACTIVE,
    FAILED,
    COMPLETED,
    CLAIMED
}

[System.Serializable]
public class AcceptedQuest
{
    public Quest quest;
    public int currentAmount;
    public QuestStatus QuestStatus;

    public string uID => quest == null ? string.Empty : quest.uID;
    public string title => quest == null ? string.Empty : quest.title;
    public int experience => quest == null ? 0 : quest.experience;
    public int gold => quest == null ? 0 : quest.gold;
    public int amount => quest == null ? 0 : quest.amount;
    public string[] targets => quest == null ? null : quest.targets;
}

public class QuestLog : MonoBehaviour
{
    public static QuestLog Instance { get; private set; }

    public List<AcceptedQuest> quests = new List<AcceptedQuest>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddQuest(Quest quest)
    {
        if (quest == null) return;

        Debug.Log("AddQuest called: " + quest.uID);
        if (quests.Exists(q => q.uID == quest.uID))
        {
            Debug.Log("Already exists!");
            return;
        }

        AcceptedQuest accepted = new AcceptedQuest
        {
            quest = quest,
            currentAmount = 0,
            QuestStatus = QuestStatus.ACTIVE
        };

        quests.Add(accepted);
        Debug.Log("Quest added! Count: " + quests.Count);
    }

    public void UpdateProgress(string targetId)
    {
        foreach (var quest in quests)
        {
            if (quest.QuestStatus != QuestStatus.ACTIVE) continue;
            if (quest.targets == null) continue;

            if (System.Array.Exists(quest.targets, t => t == targetId))
            {
                quest.currentAmount++;
                Debug.Log(quest.title + " progress: " + quest.currentAmount + "/" + quest.amount);

                if (quest.currentAmount >= quest.amount)
                {
                    quest.QuestStatus = QuestStatus.COMPLETED;
                    Debug.Log(quest.title + " completed!");
                }
                break;
            }
        }
    }
}
