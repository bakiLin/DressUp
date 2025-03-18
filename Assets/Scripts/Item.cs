using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Collider2D coll;

    private Rigidbody2D rb;

    private Transform image;

    // ����� ���� int ����������� ��� ����������� ����������� �� ��� ���������
    private IntReactiveProperty overlapProperty = new IntReactiveProperty(); 

    // ���� ��� �������� �������� � ����������� ���������� �� ��� � ����������
    private CompositeDisposable disposable = new CompositeDisposable();

    [HideInInspector]
    public bool isHeld, isScaled;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        image = transform.Find("Image"); 
        
        // ��� �������� � ����������� �������� �� ������� ��� ����������� ������ UniRx
        // ��� �������� ������� �������� ��� ����������� DoTween
        // OnTriggerEnter2D
        coll.OnTriggerEnter2DAsObservable().Subscribe(_ => 
        { 
            overlapProperty.Value++; // ����������� ���������� � ����������� �����������  
            // ��������� ��� ������ �� ������������ ������������� 
            // ���������� DoTween ��� ��������� ������ � ������ ����������� ��������
            if (!isHeld) image.DOShakePosition(0.4f, strength: new Vector3(0f, 1.5f, 0f), vibrato: 10, randomness: 0); 
        }).AddTo(disposable); // ��������� �������� � ���� disposable ��� ����������� ���������� � ����� ������

        // OnTriggerEnter2D
        // ��������� ���������� � ����������� ����������� 
        coll.OnTriggerExit2DAsObservable().Subscribe(_ => overlapProperty.Value--).AddTo(disposable); 

        // ������������� �� ����� ��������� ���������� �����������
        // ���� ������ �� ����� �������� (��������� � �������) ������ ��� ����, ����� - ����
        overlapProperty.Subscribe(_ =>
        {
            if (_ > 0) rb.linearVelocityY = 0f; 
            else rb.linearVelocityY = -5f;
        }).AddTo(disposable);
    }
}
