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
}