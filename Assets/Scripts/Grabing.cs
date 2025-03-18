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
        // При помощи raycast пытаемся найти объект класса Item 
        Collider2D[] coll = Physics2D.OverlapPointAll(GetWorldPosition());
        item = Array.Find(coll, x => x.CompareTag("Item"));

        if (item)
        {
            item.GetComponent<Item>().isHeld = true; // Обозначаем взаимодейтсвие с объектом
            offset = item.transform.position - GetWorldPosition(); // Вычисляем разницу между центром объекта и позицией клика
            // При помощи UniRx создаем подписываемся на Update и каждый кадр перемещаем объект в зависимости от положения курсора (Drag and Drop)
            Observable.EveryUpdate().Subscribe(_ => item.transform.position = GetWorldPosition() + offset).AddTo(disposable); 
        }
        else
            screenScroll.Scroll(context); // Если на пути не были обнаружены объекты, то активируем Screen Scroll
    }

    private void Release(InputAction.CallbackContext context)
    {
        // Отписываемся от событий
        disposable.Clear();

        if (item != null)
        {
            item.GetComponent<Item>().isHeld = false; // Обозначаем прекращение взаимодействия с объектом
            item = null;
        }

        // При помощи raycast пытаемся найти объект класса Holder 
        Collider2D[] coll = Physics2D.OverlapPointAll(GetWorldPosition());
        Collider2D holder = Array.Find(coll, x => x.GetComponent<Holder>());
        holder?.GetComponent<Holder>()?.MoveToPoint(); 
    }

    // Метод возвращающий положение курсора относительно камеры
    private Vector3 GetWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());
    }
}
