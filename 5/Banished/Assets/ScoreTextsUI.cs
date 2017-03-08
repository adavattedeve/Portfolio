using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTextsUI : MonoBehaviour {
    public Text scoreText;
    public Text highScoreText;
    public Text comboText;
    [Header("ComboFlashParams")]
    public Color flashColor1;
    public Color flashColor2;
    public float timeBetweenColors;
    public float timeBeforeFading;
    public float fadingSpeed;

	// Use this for initialization
	void Start () {
        GameManager.OnPriestDefeated += Refresh;
        scoreText.text = "SCORE: " + 0;
        highScoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt("HighScore");
        comboText.gameObject.SetActive(false);
	}
	

    public void Refresh(int priestsKilled)
    {
        scoreText.text = "SCORE: " + GameManager.instance.score;
        if (GameManager.instance.combo % 5 == 0)
        {
            StopAllCoroutines();
            comboText.gameObject.SetActive(true);
            comboText.text = "COMBO: " + GameManager.instance.combo;
            StartCoroutine(FlashComboTextColor());
        }
    }
    private IEnumerator FlashComboTextColor()
    {
        float totalTime = 0;
        float time = 0;

        Color textColor = comboText.color;
        textColor.a = 1;
        comboText.color = textColor;
        int whileCount = 0;
        while (totalTime < timeBeforeFading)
        {
            if (whileCount % 2 == 0)
            {
                textColor = flashColor1;
                comboText.color = textColor;
            }
            else
            {
                textColor = flashColor2;
                comboText.color = textColor;
            }
            while (time < timeBetweenColors)
            {
                yield return new WaitForEndOfFrame();
                totalTime += Time.deltaTime;
                time += Time.deltaTime;
            }

            ++whileCount;
            time = 0;
        }
        time = 0;
        whileCount = 0;
        while (textColor.a >= 0)
        {
            if (whileCount % 2 == 0)
            {
                textColor = flashColor1;
                textColor.a = comboText.color.a;
                comboText.color = textColor;
            }
            else
            {
                textColor = flashColor2;
                textColor.a = comboText.color.a;
                comboText.color = textColor;
            }
            while (time < timeBetweenColors)
            {
                yield return new WaitForEndOfFrame();
                textColor.a -= fadingSpeed * Time.deltaTime;
                comboText.color = textColor;
                time += Time.deltaTime;
            }

        }
    }
}
