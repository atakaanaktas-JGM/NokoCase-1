using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public Quest quest;
    public Dialog dialog;

    [SerializeField] private DialogManager dialogManager;

    private void OnEnable()
    {
        QuestManager.Instance?.RegisterQuestGiver(this);
    }

    private void OnDisable()
    {
        QuestManager.Instance?.UnregisterQuestGiver(this);
    }

    private void Start()
    {
        dialogManager ??= DialogManager.Instance;
    }

    public void Interact()
    {
        if (dialogManager != null)
        {
            dialogManager.StartDialog(dialog, quest, this);
        }
    }

    public void StopInteract()
    {
        if (dialogManager != null)
        {
            dialogManager.CloseDialog();
        }
    }
}
