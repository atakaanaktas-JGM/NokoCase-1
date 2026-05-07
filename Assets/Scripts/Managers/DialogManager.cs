using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogUI;
    [SerializeField] private TextMeshProUGUI dialogHeaderText;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private Button acceptQuestButton;
    [SerializeField] private Button claimRewardButton;
    [SerializeField] private QuestLog questLog;
    [SerializeField] private QuestManager questManager;

    private Quest currentQuest;
    private QuestGiver currentQuestGiver;

    private void Awake()
    {
        Instance = this;

        if (dialogUI != null)
            dialogUI.SetActive(false);

        questLog ??= QuestLog.Instance;
        questManager ??= QuestManager.Instance;

        acceptQuestButton.onClick.AddListener(AcceptQuest);
        claimRewardButton.onClick.AddListener(ClaimReward);
        acceptQuestButton.gameObject.SetActive(false);
        claimRewardButton.gameObject.SetActive(false);
    }

    public void StartDialog(Dialog dialog, Quest quest = null, QuestGiver questGiver = null)
    {
        currentQuestGiver = questGiver;

        dialogUI.SetActive(true);
        currentQuest = quest;

        if (quest == null)
        {
            SetQuestText(dialog != null ? dialog.welcomeText : string.Empty, string.Empty);
            acceptQuestButton.gameObject.SetActive(false);
            claimRewardButton.gameObject.SetActive(false);
            return;
        }

        AcceptedQuest accepted = questLog.quests.Find(q => q.uID == quest.uID);
        if (accepted != null && accepted.QuestStatus == QuestStatus.COMPLETED)
        {
            SetQuestText("Well done!", BuildQuestDescription(quest, accepted));
            claimRewardButton.gameObject.SetActive(true);
            acceptQuestButton.gameObject.SetActive(false);
        }
        else if (accepted == null)
        {
            SetQuestText(quest.title, BuildQuestDescription(quest, null));
            acceptQuestButton.gameObject.SetActive(true);
            claimRewardButton.gameObject.SetActive(false);
        }
        else
        {
            SetQuestText(quest.title, BuildQuestDescription(quest, accepted));
            acceptQuestButton.gameObject.SetActive(false);
            claimRewardButton.gameObject.SetActive(false);
        }
    }

    public void CloseDialog()
    {
        dialogUI.SetActive(false);
        acceptQuestButton.gameObject.SetActive(false);
        currentQuest = null;
    }

    private void AcceptQuest()
    {
        Debug.Log("currentQuest: " + (currentQuest == null ? "NULL" : currentQuest.title));
        Debug.Log("questLog: " + (questLog == null ? "NULL" : "OK"));
        if (currentQuest == null || questLog == null) return;

        questLog.AddQuest(currentQuest);
        acceptQuestButton.gameObject.SetActive(false);
        StartDialog(null, currentQuest, currentQuestGiver);
    }

    private void ClaimReward()
    {
        if (currentQuest == null || questLog == null) return;

        AcceptedQuest accepted = questLog.quests.Find(q => q.uID == currentQuest.uID);
        if (accepted == null) return;

        Debug.Log("Claimed! EXP: " + accepted.experience + " Gold: " + accepted.gold);

        PlayerStats.Instance?.AddExperience(accepted.experience);
        CurrencyManager.Instance?.AddCurrency(accepted.gold);

        accepted.QuestStatus = QuestStatus.CLAIMED;
        questManager?.AssignNextQuestTo(currentQuestGiver);
        CloseDialog();
    }

    private void SetQuestText(string title, string description)
    {
        if (questTitleText != null || questDescriptionText != null)
        {
            if (dialogHeaderText != null)
                dialogHeaderText.text = title;

            if (questTitleText != null)
                questTitleText.text = title;

            if (questDescriptionText != null)
                questDescriptionText.text = description;

            return;
        }

        if (dialogHeaderText != null)
        {
            dialogHeaderText.text = string.IsNullOrWhiteSpace(description)
                ? title
                : $"{title}\n{description}";
        }
    }

    private static string BuildQuestDescription(Quest quest, AcceptedQuest accepted)
    {
        if (quest == null) return string.Empty;

        string progress = string.Empty;

        if (quest.amount > 0)
        {
            int currentAmount = accepted != null ? accepted.currentAmount : 0;
            progress = $"\nProgress: {currentAmount}/{quest.amount}";
        }

        string rewards = $"\nReward: {quest.experience} EXP, {quest.gold} Gold";

        return $"{quest.description}{progress}{rewards}";
    }
}
