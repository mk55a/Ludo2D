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

    public void AddUnit(Unit unit)
    {
        unit.gameObject.transform.SetParent(transform);
        unit.gameObject.transform.position = transform.position;
    }
    public bool IsEmpty()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Unit>() != null)
            {
                // If at least one child with Unit component is found, return false
                return false;
            }
        }
        // If no child with Unit component is found, return true
        return true;

    }
}
