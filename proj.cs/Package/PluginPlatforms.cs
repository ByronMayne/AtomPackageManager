 
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;

[System.Serializable]
public class PluginPlatforms
{
    [SerializeField]
    public bool editorCompatible = true;

	[SerializeField]
    public bool anyPlatformCompatible = true;

  
	[SerializeField]
	public bool StandaloneOSXUniversalCompatible = true;
  
	[SerializeField]
	public bool StandaloneOSXIntelCompatible = true;
  
	[SerializeField]
	public bool StandaloneWindowsCompatible = true;
  
	[SerializeField]
	public bool iOSCompatible = true;
  
	[SerializeField]
	public bool PS3Compatible = true;
  
	[SerializeField]
	public bool XBOX360Compatible = true;
  
	[SerializeField]
	public bool AndroidCompatible = true;
  
	[SerializeField]
	public bool StandaloneLinuxCompatible = true;
  
	[SerializeField]
	public bool StandaloneWindows64Compatible = true;
  
	[SerializeField]
	public bool WebGLCompatible = true;
  
	[SerializeField]
	public bool WSAPlayerCompatible = true;
  
	[SerializeField]
	public bool StandaloneLinux64Compatible = true;
  
	[SerializeField]
	public bool StandaloneLinuxUniversalCompatible = true;
  
	[SerializeField]
	public bool StandaloneOSXIntel64Compatible = true;
  
	[SerializeField]
	public bool TizenCompatible = true;
  
	[SerializeField]
	public bool PSP2Compatible = true;
  
	[SerializeField]
	public bool PS4Compatible = true;
  
	[SerializeField]
	public bool PSMCompatible = true;
  
	[SerializeField]
	public bool XboxOneCompatible = true;
  
	[SerializeField]
	public bool SamsungTVCompatible = true;
  
	[SerializeField]
	public bool Nintendo3DSCompatible = true;
  
	[SerializeField]
	public bool WiiUCompatible = true;
  
	[SerializeField]
	public bool tvOSCompatible = true;

	public bool ApplyToImporter(PluginImporter importer)
    {
		bool hadChanges = false;
		if(importer.GetCompatibleWithEditor() != editorCompatible)
        {
            importer.SetCompatibleWithEditor(editorCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithAnyPlatform() != anyPlatformCompatible)
        {
            importer.SetCompatibleWithAnyPlatform(anyPlatformCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal) != StandaloneOSXUniversalCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, StandaloneOSXUniversalCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel) != StandaloneOSXIntelCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, StandaloneOSXIntelCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneWindows) != StandaloneWindowsCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, StandaloneWindowsCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.iOS) != iOSCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.iOS, iOSCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.PS3) != PS3Compatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.PS3, PS3Compatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.XBOX360) != XBOX360Compatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.XBOX360, XBOX360Compatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.Android) != AndroidCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.Android, AndroidCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneLinux) != StandaloneLinuxCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, StandaloneLinuxCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneWindows64) != StandaloneWindows64Compatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, StandaloneWindows64Compatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.WebGL) != WebGLCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.WebGL, WebGLCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.WSAPlayer) != WSAPlayerCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.WSAPlayer, WSAPlayerCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneLinux64) != StandaloneLinux64Compatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, StandaloneLinux64Compatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal) != StandaloneLinuxUniversalCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, StandaloneLinuxUniversalCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64) != StandaloneOSXIntel64Compatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, StandaloneOSXIntel64Compatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.Tizen) != TizenCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.Tizen, TizenCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.PSP2) != PSP2Compatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.PSP2, PSP2Compatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.PS4) != PS4Compatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.PS4, PS4Compatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.PSM) != PSMCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.PSM, PSMCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.XboxOne) != XboxOneCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.XboxOne, XboxOneCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.SamsungTV) != SamsungTVCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.SamsungTV, SamsungTVCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.Nintendo3DS) != Nintendo3DSCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.Nintendo3DS, Nintendo3DSCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.WiiU) != WiiUCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.WiiU, WiiUCompatible);
            hadChanges = true;
        }
		if(importer.GetCompatibleWithPlatform(BuildTarget.tvOS) != tvOSCompatible)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.tvOS, tvOSCompatible);
            hadChanges = true;
        }
		return hadChanges;
    }
}
