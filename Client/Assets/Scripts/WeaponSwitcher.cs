using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] private List<GameObject> _weaponList;

    private int _currentSlot = 0;

    private InputActionAsset _inputActions;
    private InputAction _scrollWeapon;
    private InputAction _selectWeapon1;
    private InputAction _selectWeapon2;
    private InputAction _selectWeapon3;
    private InputAction _selectWeapon4;
    private float _scrollY;

    private void Awake()
    {
        _inputActions = _controller.InputActions;

        _scrollWeapon = InputSystem.actions.FindAction("ScrollWeapon");
        _selectWeapon1 = InputSystem.actions.FindAction("SelectWeapon1");
        _selectWeapon2 = InputSystem.actions.FindAction("SelectWeapon2");
        _selectWeapon3 = InputSystem.actions.FindAction("SelectWeapon3");
        _selectWeapon4 = InputSystem.actions.FindAction("SelectWeapon4");
    }

    private void Update()
    {
        if (_selectWeapon1.WasPressedThisFrame()) SelectWeapon(0);
        if (_selectWeapon2.WasPressedThisFrame()) SelectWeapon(1);
        if (_selectWeapon3.WasPressedThisFrame()) SelectWeapon(2);
        if (_selectWeapon4.WasPressedThisFrame()) SelectWeapon(3);

        _scrollY = _scrollWeapon.ReadValue<float>();
        
        if (_scrollY > 0f)
        {
            if (_currentSlot + 1 > _weaponList.Count - 1) SelectWeapon(0);
            else SelectWeapon(_currentSlot + 1);
        } 
        else if (_scrollY < 0f)
        {
            if (_currentSlot - 1 < 0) SelectWeapon(_weaponList.Count - 1);
            else SelectWeapon(_currentSlot - 1);
        }
    }

    private void SelectWeapon(int slot)
    {
        if (_weaponList.Count - 1 < slot || slot < 0) return;

        _weaponList[_currentSlot].SetActive(false);
        _currentSlot = slot;

        _controller.EquipWeapon(_weaponList[_currentSlot], _currentSlot);
    }

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
    }
}
