using System.Collections.Generic;
using UnityEngine;

public abstract class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector] private List<TKey> _keyData = new();
    [SerializeField, HideInInspector] private List<TValue> _valueData = new();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        var dataCount = Mathf.Min(_keyData.Count, _valueData.Count);
        for (var i = 0; i < dataCount; i++)
        {
            if (ContainsKey(_keyData[i]))
            {
                LogManager.LogWarning($"Duplicate key during deserialize: {_keyData[i]}");
            }
            else
            {
                Add(_keyData[i], _valueData[i]);
            }
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        _keyData.Clear();
        _valueData.Clear();

        foreach (var pair in this)
        {
            _keyData.Add(pair.Key);
            _valueData.Add(pair.Value);
        }
    }
}