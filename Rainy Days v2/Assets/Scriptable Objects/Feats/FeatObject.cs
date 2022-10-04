using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Feat Object", menuName = "Feat")]
public class FeatObject : ScriptableObject
{
    public string title;
    public List<FeatObject> prerequisites;
}
