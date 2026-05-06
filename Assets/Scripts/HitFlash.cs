using UnityEngine;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private Renderer[] renderers;

    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashIntensity = 2f;
    [SerializeField] private float flashDuration = 0.1f;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private Material[] originalMaterials;
    private Color[] originalEmissions;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        originalMaterials = new Material[renderers.Length];
        originalEmissions = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
            originalMaterials[i].EnableKeyword("_EMISSION");
            originalEmissions[i] = originalMaterials[i].GetColor(EmissionColor);
        }
    }

    public void Flash()
    {

        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.SetColor(EmissionColor, flashColor * flashIntensity);

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.SetColor(EmissionColor, originalEmissions[i]);
    }
}