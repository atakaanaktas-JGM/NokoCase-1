using System.Collections;
using UnityEngine;

public class ArrowSystem : MonoBehaviour
{
    public static ArrowSystem Instance;

    [Header("Pools")]
    [SerializeField] private Transform[] normalPool;
    [SerializeField] private Transform[] poisonPool;

    [Header("References")]
    [SerializeField] private PlayerCombat combat;

    [Header("Settings")]
    [SerializeField] private float duration = 0.1f;

    private void Awake()
    {
        Instance = this;
    }

    // 🔥 DOĞRU POOL SEÇİMİ
    private Transform GetArrow()
    {
        Transform[] pool = combat.IsPoisonActive ? poisonPool : normalPool;

        foreach (var a in pool)
        {
            if (!a.gameObject.activeSelf)
                return a;
        }

        return null;
    }

    public void Shoot(Vector3 start, Vector3 end)
    {
        var arrow = GetArrow();
        if (arrow == null) return;

        arrow.gameObject.SetActive(true);
        StartCoroutine(MoveArrow(arrow, start, end));
    }

    private IEnumerator MoveArrow(Transform arrow, Vector3 start, Vector3 end)
    {
        float t = 0f;
        arrow.position = start;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            Vector3 pos = Vector3.Lerp(start, end, t);

            // 🔥 hafif arc (ok hissi)
            float arc = Mathf.Sin(t * Mathf.PI) * 0.5f;
            pos.y += arc;

            arrow.position = pos;

            yield return null;
        }

        arrow.gameObject.SetActive(false);
    }
}