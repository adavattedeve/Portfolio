using UnityEngine;
using System.Collections;

public class PlayerBuffTest : MonoBehaviour {

    public float asMpl = 2f;
    public float movMpl = 1.5f;
    private PlayerStats stats;
    private StatChange asBuff;
    private StatChange movBuff;
    private bool buffed;
    // Use this for initialization
    void Start () {
        stats = GetComponent<PlayerStats>();
        asBuff = new StatChange(StatType.ATTACK_SPEED, 0, asMpl);
        movBuff = new StatChange(StatType.MOVEMENT_SPEED, 0, movMpl);
        buffed = false;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.F))
        {
            if (!buffed)
            {
                buffed = true;
                stats.AddStatChange(asBuff);
                stats.AddStatChange(movBuff);
            }
            else
            {
                buffed = false;
                stats.RemoveStatChange(asBuff);
                stats.RemoveStatChange(movBuff);
            }
            
        }
	}
}
