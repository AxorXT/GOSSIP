using System.Collections;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static IEnumerator AnimateScaleIn(GameObject obj, float duration = 0.3f)
    {
        Vector3 targetScale = Vector3.one;
        obj.transform.localScale = Vector3.zero;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = targetScale;
    }

    // M�todo para hacer la transici�n de skybox (con un fade entre dos materiales)
    public static IEnumerator SwitchSkybox(Material fromSkybox, Material toSkybox, float duration = 2f)
    {
        float time = 0f;

        // Empezamos el fade
        while (time < duration)
        {
            float lerpFactor = time / duration;
            RenderSettings.skybox.Lerp(fromSkybox, toSkybox, lerpFactor);
            DynamicGI.UpdateEnvironment(); // Para que la iluminaci�n tambi�n se actualice
            time += Time.deltaTime;
            yield return null;
        }

        // Aseguramos que el skybox final se aplique completamente
        RenderSettings.skybox = toSkybox;
        DynamicGI.UpdateEnvironment();
    }
}