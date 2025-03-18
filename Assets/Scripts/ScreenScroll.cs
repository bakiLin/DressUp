using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenScroll : MonoBehaviour
{
    [SerializeField]
    private float force, xMin, xMax;

    private MobileAction mobileAction;

    private InputAction pressAction, positionAction;

    private Vector3 lastPosition, currentPosition, screenSize, offset, position;

    private CompositeDisposable disposable = new CompositeDisposable();

    private void Awake()
    {
        // ��������� ���� input action ��� ���������� ������� � ������� �������
        mobileAction = new MobileAction();
        mobileAction.Enable();

        pressAction = mobileAction.Touch.Press;
        positionAction = mobileAction.Touch.Position;

        // ���������� ������� 
        pressAction.canceled += (InputAction.CallbackContext context) => disposable.Clear();
        // ��������� ������ ������ ������������
        screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    }

    // ����� ���������� �� ����������� ������ ������������ �������
    public void Scroll(InputAction.CallbackContext context)
    {
        // ��������� ������� ������� �������
        lastPosition = Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());

        // �������� �� Update
        Observable.EveryUpdate().Subscribe(_ =>
        {
            // ���������� ������ ����������� �������
            currentPosition = Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());
            offset = new Vector3(currentPosition.x - lastPosition.x, 0f, 0f);
            Camera.main.transform.position -= Time.deltaTime * force * offset;

            // ������������ ��������� ������ ������� �� ��� X
            position = Camera.main.transform.position;
            position.x = Mathf.Clamp(position.x, xMin + screenSize.x, xMax - screenSize.x);
            Camera.main.transform.position = position;

            lastPosition = currentPosition;

        }).AddTo(disposable);
    }
}
