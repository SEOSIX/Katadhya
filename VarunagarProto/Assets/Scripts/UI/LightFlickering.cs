using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlickering : MonoBehaviour
{
    private Light2D light;

    // DEFAULT VALUES
    float defaultIntensity;
    Vector2 defaultUniformScale;

    [Header("Intensity parameters")]
    [SerializeField] private float intensityVariationRange = 0.5f;
    [SerializeField] private float intensityVariationSpeed = 1;
    [Header("Scale parameters")]
    [SerializeField] private Vector2 scaleVariationRange = new Vector2(0.5f, 0.5f);
    [SerializeField] private float scaleVariationSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        if (light == null) { return; }
        defaultIntensity = light.intensity;
        defaultUniformScale.x = transform.localScale.x;
        defaultUniformScale.y = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // INTENSITY
        float randomValue1 = Mathf.PerlinNoise(Time.time * intensityVariationSpeed, 0f);
        float finalIntensity = Remap(randomValue1, 0, 1, defaultIntensity - intensityVariationRange, defaultIntensity + intensityVariationRange);
        light.intensity = finalIntensity;

        // SCALE
        float randomValue2 = Mathf.PerlinNoise(Time.time * scaleVariationSpeed, 100f);
        Vector3 finalScale = new Vector3(Remap(randomValue2, 0, 1, defaultUniformScale.x - scaleVariationRange.x, defaultUniformScale.x + scaleVariationRange.x),
                                        Remap(randomValue2, 0, 1, defaultUniformScale.y - scaleVariationRange.y, defaultUniformScale.y + scaleVariationRange.y),
                                        1);
        transform.localScale = finalScale;

    }

    float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
