using System.Collections;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material blendMaterial; // El material con el shader de arriba
    public float transitionDuration = 5f;

    private float blendValue = 0f;
    private bool transitioning = false;

    public void StartBlend(bool toNight)
    {
        if (!transitioning)
            StartCoroutine(BlendSkybox(toNight ? 1f : 0f));
    }

    private IEnumerator BlendSkybox(float target)
    {
        transitioning = true;
        float start = blendValue;
        float time = 0f;

        while (time < transitionDuration)
        {
            blendValue = Mathf.Lerp(start, target, time / transitionDuration);
            blendMaterial.SetFloat("_Blend", blendValue);
            DynamicGI.UpdateEnvironment();
            time += Time.deltaTime;
            yield return null;
        }

        blendValue = target;
        blendMaterial.SetFloat("_Blend", target);

        // Refuerza el skybox aplicado y actualiza iluminación global correctamente
        RenderSettings.skybox = blendMaterial;
        DynamicGI.UpdateEnvironment();

        transitioning = false;
    }

    void Start()
    {
        blendValue = 0f; // Día al inicio
        blendMaterial.SetFloat("_Blend", blendValue);
        RenderSettings.skybox = blendMaterial;
        DynamicGI.UpdateEnvironment(); // Asegura que desde el inicio esté todo bien aplicado
    }
}
