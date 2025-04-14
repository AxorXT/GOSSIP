using System.Collections;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material skyboxDay;
    public Material skyboxNight;
    private float cycleDuration = 10f; // Duración de cada ciclo de día y noche
    private float transitionDuration = 20f; // Duración de la transición (desvanecimiento)

    // Cambié el modificador de acceso a public
    public void StartDayNightCycle()
    {
        StartCoroutine(SkyboxCycle());
    }

    private IEnumerator SkyboxCycle()
    {
        while (true)
        {
            // Transición de día a noche
            yield return StartCoroutine(SwitchSkybox(skyboxDay, skyboxNight, transitionDuration));
            yield return new WaitForSeconds(cycleDuration);

            // Transición de noche a día
            yield return StartCoroutine(SwitchSkybox(skyboxNight, skyboxDay, transitionDuration));
            yield return new WaitForSeconds(cycleDuration);
        }
    }

    // Método para hacer la transición suave entre dos skyboxes
    private IEnumerator SwitchSkybox(Material fromSkybox, Material toSkybox, float duration)
    {
        float time = 0f;
        // Mientras no haya alcanzado la duración de la transición, interpolamos
        while (time < duration)
        {
            float lerpFactor = time / duration;  // Factores de interpolación
            RenderSettings.skybox.Lerp(fromSkybox, toSkybox, lerpFactor); // Interpolación suave entre skyboxes
            DynamicGI.UpdateEnvironment(); // Actualiza la iluminación
            time += Time.deltaTime;
            yield return null; // Esperamos un frame
        }

        // Cuando termina la interpolación, aplicamos el skybox final
        RenderSettings.skybox = toSkybox;
        DynamicGI.UpdateEnvironment(); // Asegura que la iluminación se actualice correctamente
    }
}
