using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Quest Database")]
public class QuestDatabaseSO : ScriptableObject
{
    public List<Quest> quests = new List<Quest>();
}
