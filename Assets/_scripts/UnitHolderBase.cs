using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHolderBase : MonoBehaviour
{
    public Unit InstantiateUnit(GameObject unit)
    {
        GameObject unitGameObject = Instantiate(unit, transform.position, Quaternion.identity, transform);
        Unit unitComponent = unitGameObject.GetComponent<Unit>();
        return unitComponent;
    }

    public bool IsEmpty()
    {
        return true; 

    }
}
