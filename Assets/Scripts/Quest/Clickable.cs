using UnityEngine;

public class Clickable : MonoBehaviour
{
    private QuestGiver questGiver;

    private void Awake()
    {
        questGiver = GetComponent<QuestGiver>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questGiver?.Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questGiver?.StopInteract();
        }
    }
}