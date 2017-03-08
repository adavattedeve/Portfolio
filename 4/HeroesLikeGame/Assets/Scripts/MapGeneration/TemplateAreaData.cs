using UnityEngine;
using System.Collections;

public enum ConnectionType { OPEN, CLOSED, PATH, GUARDEDPATH }

[System.Serializable]
public class TemplatePathway
{
    public ConnectionType type = ConnectionType.GUARDEDPATH;
    [HideInInspector]public TerrainType unwalkableType;
    public IntVector2 to;
    //public float difficulty;
    public TemplatePathway()
    {
        type = ConnectionType.CLOSED;
        to = null;
    }
}

[System.Serializable]
public class TemplateAreaData  {
    public AreaType areaType;
    public TemplatePathway[] pathwayTemplates;
    //public float difficulty;
    //   - area size??? plzz..
    //- loot density
    //- loot quality
    //- enemy density(how much enemies to protect loot)
    //- enemy quality(how hard enemies)
}
