using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[InitializeOnLoad]
public class WUDATADEFINE
{
	static WUDATADEFINE()
	{
		BuildTargetGroup btg = EditorUserBuildSettings.selectedBuildTargetGroup;
		string defines_field = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
		List<string> defines = new List<string>(defines_field.Split(';'));
		if (!defines.Contains("WUDATA"))
		{
			defines.Add("WUDATA");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, string.Join(";", defines.ToArray()));
		}
	}
}


