using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHolderBase : MonoBehaviour
{
    // Instantiate a unit GameObject and return its Unit component
    public Unit InstantiateUnit(GameObject unitPrefab)
    {
        GameObject unitGameObject = Instantiate(unitPrefab, transform.position, Quaternion.identity, transform);
        return unitGameObject.GetComponent<Unit>();
    }

    // Add a unit to the unit holder
    public void AddUnit(Unit unit)
    {
        // Set the unit's parent to this unit holder and position it at the holder's position
        unit.transform.SetParent(transform);
        unit.transform.position = transform.position;
    }

    // Check if the unit holder is empty
    public bool IsEmpty()
    {
        // Check if any child GameObject of this transform has a Unit component
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
