using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// References all of the entities and handles their creation and destruction
/// </summary>
public class SmallEntitiesManager : MonoBehaviour
{
    [field:SerializeField] public List<SmallEntity> Entities {get; private set;}
    
}
