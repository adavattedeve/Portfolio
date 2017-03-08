using UnityEngine;
using System.Collections;

public class RitualProgresUI : MonoBehaviour {
    public Priest priest;
    public RectTransform slider;
    private float sliderWidth;
    private Vector2 originalAnchoredPosition;
    void Awake()
    {
        priest.OnRitualProgresChange += RefreshSlider;

        sliderWidth = slider.offsetMax.x - slider.offsetMin.x;
        originalAnchoredPosition = slider.anchoredPosition;
    }
    public void RefreshSlider(float progres)
    {
        if (progres == 0 || progres == 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            slider.anchoredPosition = new Vector2(originalAnchoredPosition.x - (1 - progres) * sliderWidth, slider.anchoredPosition.y);
        }

    }

}
