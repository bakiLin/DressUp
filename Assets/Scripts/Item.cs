using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Collider2D coll;

    private Rigidbody2D rb;

    private Transform image;

    // Класс типа int необходимый для возможности подписаться на его изменения
    private IntReactiveProperty overlapProperty = new IntReactiveProperty(); 

    // Поле для хранения подписок и возможности отписаться от них в дальнейшем
    private CompositeDisposable disposable = new CompositeDisposable();

    [HideInInspector]
    public bool isHeld, isScaled;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        image = transform.Find("Image"); 
        
        // Для гибкости и возможности подписки на события был использован плагин UniRx
        // Для создания простых анимаций был использован DoTween
        // OnTriggerEnter2D
        coll.OnTriggerEnter2DAsObservable().Subscribe(_ => 
        { 
            overlapProperty.Value++; // Увеличиваем переменную с количеством пересечений  
            // Проверяем что объект не удерживается пользователем 
            // Используем DoTween для симуляции тряски и задаем необходимые атрибуты
            if (!isHeld) image.DOShakePosition(0.4f, strength: new Vector3(0f, 1.5f, 0f), vibrato: 10, randomness: 0); 
        }).AddTo(disposable); // Сохраняем подписку в поле disposable для возможности отписаться в любой момент

        // OnTriggerEnter2D
        // Уменьшаем переменную с количеством пересечений 
        coll.OnTriggerExit2DAsObservable().Subscribe(_ => overlapProperty.Value--).AddTo(disposable); 

        // Подписываемся на любое изменение количества пересечений
        // Если объект не имеет коллизий (находится в воздухе) роняем его вниз, иначе - стоп
        overlapProperty.Subscribe(_ =>
        {
            if (_ > 0) rb.linearVelocityY = 0f; 
            else rb.linearVelocityY = -5f;
        }).AddTo(disposable);
    }
}
