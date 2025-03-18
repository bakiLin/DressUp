using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabing : MonoBehaviour
{
    [SerializeField]
    private ScreenScroll screenScroll;

    private MobileAction mobileAction;

    private InputAction pressAction, positionAction;

    private Collider2D item;

    private Vector3 offset;

    private CompositeDisposable disposable = new CompositeDisposable();

    private void Awake()
    {
        // Объявляем поле input action для считывания касаний и позиции касаний
        mobileAction = new MobileAction(); 
        mobileAction.Enable(); 

        pressAction = mobileAction.Touch.Press; 
        positionAction = mobileAction.Touch.Position;

        pressAction.started += Press;
        pressAction.canceled += Release;
    }

    private void Press(InputAction.CallbackContext context)
    {
        // ��� ������ raycast �������� ����� ������ ������ Item 
        Collider2D[] coll = Physics2D.OverlapPointAll(GetWorldPosition());
        item = Array.Find(coll, x => x.CompareTag("Item"));

        if (item)
        {
            item.GetComponent<Item>().isHeld = true; // ���������� �������������� � ��������
            offset = item.transform.position - GetWorldPosition(); // ��������� ������� ����� ������� ������� � �������� �����
            // ��� ������ UniRx ������� ������������� �� Update � ������ ���� ���������� ������ � ����������� �� ��������� ������� (Drag and Drop)
            Observable.EveryUpdate().Subscribe(_ => item.transform.position = GetWorldPosition() + offset).AddTo(disposable); 
        }
        else
            screenScroll.Scroll(context); // ���� �� ���� �� ���� ���������� �������, �� ���������� Screen Scroll
    }

    private void Release(InputAction.CallbackContext context)
    {
        // ������������ �� �������
        disposable.Clear();

        if (item != null)
        {
            item.GetComponent<Item>().isHeld = false; // ���������� ����������� �������������� � ��������
            item = null;
        }

        // ��� ������ raycast �������� ����� ������ ������ Holder 
        Collider2D[] coll = Physics2D.OverlapPointAll(GetWorldPosition());
        Collider2D holder = Array.Find(coll, x => x.GetComponent<Holder>());
        holder?.GetComponent<Holder>()?.MoveToPoint(); 
    }

    // ����� ������������ ��������� ������� ������������ ������
    private Vector3 GetWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());
    }
}
