using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    [SerializeField] private List<GameObject> _weaponList;
    private int _currentSlot = 0;

    public void SetActive(int slot)
    {
        _weaponList[_currentSlot].SetActive(false);
        _currentSlot = slot;
        _weaponList[_currentSlot].SetActive(true);
    }
}
