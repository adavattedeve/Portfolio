using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool rotate = false;
    public float mouseOverRotationSpeed = 100;
    public float mouseOffRotationSpeed = 20;
    public float mouseOverScaleSpeed = 10;
    public float mouseOffScaleSpeed = 4;
    public float maxErrorAngle = 0.1f;
    public float maxErrorDistance = 0.001f;
    private Vector3 defaultScale;

    private RectTransform rect;
    private bool mouseOnButton;
    public bool MouseOnButton { set { mouseOnButton = value; } }

    //private Sprite normal;
    //private Sprite pressed;

    private Text buttonText;

	void Start () {
        buttonText = GetComponentInChildren<Text>();
        if (buttonText == null)
        {
            Debug.Log("no text in children, wtff, his name IIIIS: " + gameObject.name);
        }
        buttonText.color = UIManager.instance.buttonTextColor;
        buttonText.font = UIManager.instance.buttonTextFont;
        mouseOnButton = false;
        rect = GetComponent<RectTransform>();
        defaultScale = rect.localScale;
 
    }

    void OnEnable()
    {
        mouseOnButton = false;
    }
    void Update()
    {
        Scale(mouseOnButton);
    }
    private void Scale(bool up)
    {
        if (up)
        {
            if (Vector3.Distance(rect.localScale, Vector3.one) > maxErrorDistance)
            {
                rect.localScale = Vector3.MoveTowards(rect.localScale, Vector3.one, mouseOverScaleSpeed * Time.deltaTime);
            }

        }
        else {
            if (Vector3.Distance(rect.localScale, defaultScale) > maxErrorDistance)
            {
                rect.localScale = Vector3.MoveTowards(rect.localScale, defaultScale, mouseOffScaleSpeed * Time.deltaTime);
            }
        }
    }
    public void OnPointerEnter(PointerEventData e)
    {
        AudioManager.instance.MouseOnButton();
        mouseOnButton = true;
    }
    public void OnPointerExit(PointerEventData e)
    {
        mouseOnButton = false;
    }
    public void OnPointerDown(PointerEventData e)
    {
        mouseOnButton = false;
        rect.localScale = defaultScale;

    }
    public void OnPointerUp(PointerEventData e)
    {
        AudioManager.instance.ButtonClicked();
        OnPointerEnter(null);
    }
}
