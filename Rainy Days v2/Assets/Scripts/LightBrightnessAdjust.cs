using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBrightnessAdjust : MonoBehaviour
{
    private Light2D globalLight;
    private OptionsMenuFunctions options;
    private float startLightIntensity;
    public static float translateValue = 0.2f;

    void Start()
    {
        FindStartValue();
    }

    public void FindStartValue()
    {
        StartCoroutine(FindStartValueCo());
    }

    private IEnumerator FindStartValueCo()
    {
        yield return new WaitUntil(() => (globalLight = GetComponent<Light2D>()) != null);
        startLightIntensity = globalLight.intensity;
        LightAdjust();
    }

    public void LightAdjust()
    {
        StartCoroutine(FindAdjustValue());
    }

    private IEnumerator FindAdjustValue()
    {
        yield return new WaitUntil(() => (options = FindObjectOfType<OptionsMenuFunctions>()) != null);
        globalLight.intensity = startLightIntensity + TranslateSlider(options.brightnessSlider.normalizedValue);
    }

    private float TranslateSlider(float sliderNormalizedValue)
    {
        return sliderNormalizedValue * translateValue;
    }
}
