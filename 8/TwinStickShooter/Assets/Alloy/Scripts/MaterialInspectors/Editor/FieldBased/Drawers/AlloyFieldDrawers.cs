// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

using System.Linq;
using UnityEditor;
using UnityEngine;

public class AlloyDefaultDrawer : AlloyFieldDrawer
{
    public override void Draw(AlloyFieldDrawerArgs args) {
        PropField(DisplayName);
    }

    public AlloyDefaultDrawer(AlloyInspectorBase editor, MaterialProperty property) : base(editor, property) {
    }
}

public class AlloyLightmapEmissionDrawer : AlloyFieldDrawer {
    public override void Draw(AlloyFieldDrawerArgs args) {
        args.Editor.MatEditor.LightmapEmissionProperty();
        
        foreach (var material in args.Materials) {
            // Setup lightmap emissive flags
            MaterialGlobalIlluminationFlags flags = material.globalIlluminationFlags;
            if ((flags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != 0) {
                flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                
            
                material.globalIlluminationFlags = flags;
            }
        }
    }

    public AlloyLightmapEmissionDrawer(AlloyInspectorBase editor, MaterialProperty property) : base(editor, property) {
    }
}

public class AlloyRenderingModeDrawer : AlloyBlendModeDropdownDrawer {    
    private enum RenderingMode {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    private static readonly BlendModeOptionConfig[] s_renderingModes = {
        new BlendModeOptionConfig() {
            Type = (int)RenderingMode.Opaque,
            Keyword = "",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.Zero,
            ZWrite = 1,
            RenderQueue = -1
        },
        new BlendModeOptionConfig() {
            Type = (int)RenderingMode.Cutout,
            Keyword = "_ALPHATEST_ON",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.Zero,
            ZWrite = 1,
            RenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest,
        },
        new BlendModeOptionConfig() {
            Type = (int)RenderingMode.Fade,
            Keyword = "_ALPHABLEND_ON",
            SrcBlend = UnityEngine.Rendering.BlendMode.SrcAlpha,
            DstBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha,
            ZWrite = 0,
            RenderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent
        },
        new BlendModeOptionConfig() {
            Type = (int)RenderingMode.Transparent,
            Keyword = "_ALPHAPREMULTIPLY_ON",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha,
            ZWrite = 0,
            RenderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent
        },
    };

    public AlloyRenderingModeDrawer(AlloyInspectorBase editor, MaterialProperty property) 
        : base(editor, property, s_renderingModes) {
    }
}

public class AlloySpeedTreeGeometryTypeDrawer : AlloyBlendModeDropdownDrawer {
    private enum SpeedTreeGeometryType {
        Branch,
        BranchDetail,
        Frond,
        Leaf,
        Mesh,
    }

    private static readonly BlendModeOptionConfig[] s_geometryTypes = {
        new BlendModeOptionConfig() {
            Type = (int)SpeedTreeGeometryType.Branch,
            Keyword = "GEOM_TYPE_BRANCH",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.Zero,
            ZWrite = 1,
            RenderQueue = -1
        },
        new BlendModeOptionConfig() {
            Type = (int)SpeedTreeGeometryType.BranchDetail,
            Keyword = "GEOM_TYPE_BRANCH_DETAIL",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.Zero,
            ZWrite = 1,
            RenderQueue = -1
        },
        new BlendModeOptionConfig() {
            Type = (int)SpeedTreeGeometryType.Frond,
            Keyword = "GEOM_TYPE_FROND",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.Zero,
            ZWrite = 1,
            RenderQueue = -1
        },
        new BlendModeOptionConfig() {
            Type = (int)SpeedTreeGeometryType.Leaf,
            Keyword = "GEOM_TYPE_LEAF",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.Zero,
            ZWrite = 1,
            RenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest
        },
        new BlendModeOptionConfig() {
            Type = (int)SpeedTreeGeometryType.Mesh,
            Keyword = "GEOM_TYPE_MESH",
            SrcBlend = UnityEngine.Rendering.BlendMode.One,
            DstBlend = UnityEngine.Rendering.BlendMode.Zero,
            ZWrite = 1,
            RenderQueue = -1
        }, 
    };
    
    public AlloySpeedTreeGeometryTypeDrawer(AlloyInspectorBase editor, MaterialProperty property) 
        : base(editor, property, s_geometryTypes)
    {
    }
}

public class AlloyDecalSortOrderDrawer : AlloyFieldDrawer {
    private const float PostAlphaTestQueue = 2450.0f + 1.0f;

    public override void Draw(AlloyFieldDrawerArgs args) {
        float sortOrder = Serialized.floatValue - PostAlphaTestQueue;

        // Snap to integers.
        EditorGUI.BeginChangeCheck();
        sortOrder = (int)EditorGUILayout.Slider(DisplayName, sortOrder, 0, 20, GUILayout.MinWidth(20.0f));

        if (EditorGUI.EndChangeCheck()) {
            Serialized.floatValue = sortOrder + PostAlphaTestQueue;

            foreach (var material in args.Materials) {
                material.renderQueue = (int)Serialized.floatValue;
            }
        }
    }

    public AlloyDecalSortOrderDrawer(AlloyInspectorBase editor, MaterialProperty property) : base(editor, property) {
    }
}

public class AlloyColorParser : AlloyFieldParser{
    protected override AlloyFieldDrawer GenerateDrawer(AlloyInspectorBase editor) {
        var ret = new AlloyColorDrawer(editor, MaterialProperty);
        return ret;
    }
    
    public AlloyColorParser(MaterialProperty field) : base(field) {
    }
}

public class AlloyColorDrawer : AlloyFieldDrawer {
    public override void Draw(AlloyFieldDrawerArgs args) {
        MaterialPropField(DisplayName, args);
    }

    public AlloyColorDrawer(AlloyInspectorBase editor, MaterialProperty property) : base(editor, property) {
    }
}
