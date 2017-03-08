using UnityEngine;
using System.Collections;

public class HealthBarUI : MonoBehaviour {
    public Health health;
    public RectTransform slider;
    private float sliderWidth;
    private Vector2 originalAnchorPos;
    void Awake()
    {
        health.OnHealthChange += RefreshSlider;

        sliderWidth = slider.anchorMax.x - slider.anchorMin.x;
        originalAnchorPos = new Vector2(slider.anchorMin.x, slider.anchorMax.x);
    }
    public void RefreshSlider(float health)
    {
        Debug.Log(health);
        if (slider != null)
        {
            slider.anchorMin = new Vector2(originalAnchorPos.x - (1 - health) * sliderWidth, slider.anchorMin.y);
            slider.anchorMax = new Vector2(originalAnchorPos.y - (1 - health) * sliderWidth, slider.anchorMax.y);
            slider.anchoredPosition = Vector2.zero;
        }
    }

}
