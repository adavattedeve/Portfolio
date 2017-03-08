using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    public Color buttonTextColor;
    public Font buttonTextFont;

    public Color otherTextColor;
    public Font otherTextFont;
    public Texture2D cursor;

    //public Game
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //Cursor.SetCursor(cursor, new Vector2(64, 64), CursorMode.Auto);
            StartCoroutine(SetCursor());
        }
    }

    private IEnumerator SetCursor()
    {
        yield return null;
        Cursor.SetCursor(cursor, new Vector2(64, 64), CursorMode.Auto);
    }
}
