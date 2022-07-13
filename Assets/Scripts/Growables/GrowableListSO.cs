using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GrowableListSO : ScriptableObject
{
    public List<GrowableData> GrowableList => growableList;

    [SerializeField] private List<GrowableData> growableList;
}
