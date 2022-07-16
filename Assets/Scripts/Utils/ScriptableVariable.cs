using System;
using UnityEngine;

[Serializable]
public class ScriptableVariable<T> : ScriptableObject
{
    [SerializeField] private T serializedValue;
    private T runtimeValue;
	private bool initialized;

    public T Value
    {
        get
        {
            if (!initialized)
            {
                runtimeValue = serializedValue;
                initialized = true;
            }
            return runtimeValue;
        }

        set
        {
            initialized = true;
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            runtimeValue = value;
        }
    }
}

[CreateAssetMenu(menuName = "ScriptableVariables/Int")]
public class ScriptableInt : ScriptableVariable<int>
{
}

[CreateAssetMenu(menuName = "ScriptableVariables/Float")]
public class ScriptableFloat : ScriptableVariable<float>
{
}