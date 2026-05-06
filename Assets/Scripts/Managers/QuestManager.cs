using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private QuestDatabaseSO questDatabase;
    [SerializeField] private QuestLog questLog;
    [SerializeField] private QuestGiver[] questGivers;

    private readonly List<QuestGiver> registeredQuestGivers = new List<QuestGiver>();

    public List<Quest> quests = new List<Quest>();

    private void Awake()
    {
        Instance = this;
        questLog ??= QuestLog.Instance;
        LoadQuestsFromDatabase();
    }

    private void Start()
    {
        AssignQuests();
    }

    public void RegisterQuestGiver(QuestGiver questGiver)
    {
        if (questGiver != null && !registeredQuestGivers.Contains(questGiver))
            registeredQuestGivers.Add(questGiver);
    }

    public void UnregisterQuestGiver(QuestGiver questGiver)
    {
        registeredQuestGivers.Remove(questGiver);
    }

    public void AssignNextQuestTo(QuestGiver questGiver)
    {
        if (questGiver == null) return;

        UniqueID uniqueID = questGiver.GetComponent<UniqueID>();
        if (uniqueID == null) return;

        string giverId = uniqueID.Get_uID;
        Debug.Log("AssignNextQuestTo: " + giverId);

        List<Quest> giverQuests = quests.FindAll(q => q != null && q.questGiver == giverId);
        Debug.Log("GiverQuests count: " + giverQuests.Count);

        foreach (var quest in giverQuests)
        {
            bool alreadyDone = questLog != null && questLog.quests.Exists(q => q.uID == quest.uID &&
                (q.QuestStatus == QuestStatus.CLAIMED || q.QuestStatus == QuestStatus.COMPLETED));

            Debug.Log(quest.uID + " alreadyDone: " + alreadyDone);
            if (!alreadyDone)
            {
                questGiver.quest = quest;
                Debug.Log("Assigned: " + quest.title);
                return;
            }
        }

        questGiver.quest = null;
    }

    private void AssignQuests()
    {
        IEnumerable<QuestGiver> givers =
            questGivers != null && questGivers.Length > 0
            ? questGivers
            : registeredQuestGivers;

        foreach (var questGiver in givers)
        {
            AssignNextQuestTo(questGiver);
        }
    }

    private void LoadQuestsFromDatabase()
    {
        if (questDatabase == null)
        {
            Debug.LogWarning("QuestDatabaseSO is not assigned. Using inspector quest list.");
            return;
        }

        quests = new List<Quest>(questDatabase.quests);
        Debug.Log("Loaded quests: " + quests.Count);
    }
}
