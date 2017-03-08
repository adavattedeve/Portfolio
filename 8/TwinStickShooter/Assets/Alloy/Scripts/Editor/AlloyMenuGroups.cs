// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

using UnityEditor;
using UnityEngine;

public static class AlloyMenuGroups {
    [MenuItem(AlloyUtils.MenubarPath + "Documentation", false, 100)]
    static void Documentation() {
        Application.OpenURL("https://alloy.rustltd.com/documentation");
    }

    [MenuItem(AlloyUtils.MenubarPath + "Samples", false, 100)]
    static void Samples() {
        Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/43687");
    }

    [MenuItem(AlloyUtils.MenubarPath + "Contact", false, 100)]
    static void Contact() {
        Application.OpenURL("https://alloy.rustltd.com/contact");
    }

    [MenuItem(AlloyUtils.MenubarPath + "About", false, 100)]
    static void About() {
        Application.OpenURL("https://alloy.rustltd.com/");
    }
}
