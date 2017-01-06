using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class AtomAssembly
{
    [SerializeField]
    private string m_AssemblyName;

    [SerializeField]
    private string[] m_CompiledScripts;

    [SerializeField]
    private string[] m_References;

    [SerializeField]
    private bool m_EditorCompatible;

    [SerializeField]
    private bool m_CompatibleWithAnyPlatform;

    [SerializeField]
    private BuildTarget m_BuildTargets;

    public string assemblyName
    {
        get { return m_AssemblyName; }
        set { m_AssemblyName = value; }
    }
    public string[] compiledScripts
    {
        get { return m_CompiledScripts; }
        set { m_CompiledScripts = value; }
    }
    public string[] references
    {
        get { return m_References; }
        set { m_References = value; }
    }

    public bool EditorCompatible
    {
        get { return m_EditorCompatible; }
        set { m_EditorCompatible = value; }
    }

    public bool CompatibleWithAnyPlatform
    {
        get { return m_CompatibleWithAnyPlatform; }
        set { m_CompatibleWithAnyPlatform = value; }
    }

    public BuildTarget buildTargets
    {
        get { return m_BuildTargets; }
        set { m_BuildTargets = value; }
    }
}

public class AtomMeta : ScriptableObject
{
    [SerializeField]
    private string m_Version;

    [SerializeField]
    private AtomAssembly[] m_Assemblies;

    public string version
    {
        get { return m_Version; }
        set { m_Version = value; }
    }

    public AtomAssembly[] assemblies
    {
        get { return m_Assemblies; }
        set { m_Assemblies = value; }
    }
}
