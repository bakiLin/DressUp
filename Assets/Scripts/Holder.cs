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
        // ��������� ������ ����� Item
        itemTransform = collision.GetComponent<Item>() ? collision.GetComponent<Item>().transform : null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (itemTransform & normalScale != Vector3.zero)
        {
            // ������������ �� ��������
            positionTween.Kill();
            scaleTween.Kill();
            
            itemTransform.localScale = normalScale; // ���������� ������� ������� ������
            itemTransform.GetComponent<Item>().isScaled = false; // ���������� ��������� ��������� �������
            itemTransform = null;
        }
    }

    // ����� ������������ ������ � ��������� �����
    // �������� ��������� ��� ������ DoTween
    public void MoveToPoint()
    {
        if (itemTransform != null)
        {
            if (!itemTransform.GetComponent<Item>().isScaled) // �������� �� ��������� ��������� 
            {
                itemTransform.GetComponent<Item>().isScaled = true; 

                normalScale = itemTransform.transform.localScale; // ��������� ����������� ������ �������

                // �������� ����������� �������
                positionTween = itemTransform.DOMove(transform.localPosition + position, 1f);
                // �������� ��������� ������� �������
                scaleTween = itemTransform.DOScale(itemTransform.localScale * scaleMultiply, 1f);
            }
        }
    }
}
