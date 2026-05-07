using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenDamageFlash : MonoBehaviour
{
    public static ScreenDamageFlash Instance { get; private set; }

    public Image[] cornerImages;
    public float flashAmount = 0.5f;
    public float fadeSpeed = 3f;

    private Coroutine routine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 🔥 başlangıçta tamamen kapalı
        SetAlpha(0f);
    }

    public void Flash()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 🔥 anlık göster
        SetAlpha(flashAmount);

        // 🔥 fade out
        float alpha = flashAmount;

        while (alpha > 0.01f)
        {
            alpha = Mathf.Lerp(alpha, 0f, Time.deltaTime * fadeSpeed);
            SetAlpha(alpha);
            yield return null;
        }

        // 🔥 tamamen sıfırla (çok önemli)
        SetAlpha(0f);
    }

    private void SetAlpha(float a)
    {
        foreach (var img in cornerImages)
        {
            if (img == null) continue;

            Color c = img.color;
            c.a = a;
            img.color = c;
        }
    }
}
