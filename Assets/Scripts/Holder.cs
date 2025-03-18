using UnityEngine;
using DG.Tweening;

public class Holder : MonoBehaviour
{
    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private float scaleMultiply;

    private Transform itemTransform;

    private Vector3 normalScale;

    private Tween positionTween, scaleTween;
     
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Сохраняем объект класс Item
        itemTransform = collision.GetComponent<Item>() ? collision.GetComponent<Item>().transform : null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (itemTransform & normalScale != Vector3.zero)
        {
            // Отписываемся от анимации
            positionTween.Kill();
            scaleTween.Kill();
            
            itemTransform.localScale = normalScale; // Возвращаем объекту прежний размер
            itemTransform.GetComponent<Item>().isScaled = false; // Обозначаем отстутвие изменений размера
            itemTransform = null;
        }
    }

    // Метод перемещающий объект в указанную точку
    // Анимация выполнена при помощи DoTween
    public void MoveToPoint()
    {
        if (itemTransform != null)
        {
            if (!itemTransform.GetComponent<Item>().isScaled) // Проверка на отстутвие изменений 
            {
                itemTransform.GetComponent<Item>().isScaled = true; 

                normalScale = itemTransform.transform.localScale; // Сохраняем изначальный размер объекта

                // Анимация перемещения объекта
                positionTween = itemTransform.DOMove(transform.localPosition + position, 1f);
                // Анимация изменения размера объекта
                scaleTween = itemTransform.DOScale(itemTransform.localScale * scaleMultiply, 1f);
            }
        }
    }
}
