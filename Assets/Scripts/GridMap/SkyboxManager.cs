using System.Collections;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material skyboxDay;
    public Material skyboxNight;
    private float cycleDuration = 10f; // Duraci�n de cada ciclo de d�a y noche
    private float transitionDuration = 20f; // Duraci�n de la transici�n (desvanecimiento)

    // Cambi� el modificador de acceso a public
    public void StartDayNightCycle()
    {
        StartCoroutine(SkyboxCycle());
    }

    private IEnumerator SkyboxCycle()
    {
        while (true)
        {
            // Transici�n de d�a a noche
            yield return StartCoroutine(SwitchSkybox(skyboxDay, skyboxNight, transitionDuration));
            yield return new WaitForSeconds(cycleDuration);

            // Transici�n de noche a d�a
            yield return StartCoroutine(SwitchSkybox(skyboxNight, skyboxDay, transitionDuration));
            yield return new WaitForSeconds(cycleDuration);
        }
    }

    // M�todo para hacer la transici�n suave entre dos skyboxes
    private IEnumerator SwitchSkybox(Material fromSkybox, Material toSkybox, float duration)
    {
        float time = 0f;
        // Mientras no haya alcanzado la duraci�n de la transici�n, interpolamos
        while (time < duration)
        {
            float lerpFactor = time / duration;  // Factores de interpolaci�n
            RenderSettings.skybox.Lerp(fromSkybox, toSkybox, lerpFactor); // Interpolaci�n suave entre skyboxes
            DynamicGI.UpdateEnvironment(); // Actualiza la iluminaci�n
            time += Time.deltaTime;
            yield return null; // Esperamos un frame
        }

        // Cuando termina la interpolaci�n, aplicamos el skybox final
        RenderSettings.skybox = toSkybox;
        DynamicGI.UpdateEnvironment(); // Asegura que la iluminaci�n se actualice correctamente
    }
}
