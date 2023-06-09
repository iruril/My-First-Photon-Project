using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region Private Fields
    [SerializeField]
    private float distance = 6.5f;

    [SerializeField]
    private float height = 9.0f;

    [SerializeField]
    private Vector3 cameraOffset = Vector3.zero;

    [SerializeField]
    private bool followOnStart = false;

    Transform tracking;
    bool isFollowing;
    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }


    //Late �� �Է��ϸ� ��� ��ũ��Ʈ�� Update �� �Ϸ��� �ڿ� �ش� ������ ó���Ѵٴ� ���̴�.
    //(Late�� �Է����� ������ Ư��PC ������ ����� ���� �߻�)
    void LateUpdate()
    {
        
        if (tracking == null && isFollowing)
        {
            OnStartFollowing();
        }

        if (isFollowing)
        {
            Follow();
        }
    }
    #endregion

    #region Public Methods
    public void OnStartFollowing()
    {
        tracking = this.transform;
        isFollowing = true;
    }
    #endregion

    #region Private Methods
    void Follow()
    {
        Camera.main.transform.eulerAngles = new Vector3(60, 0, 0);
        Camera.main.transform.position = tracking.position -
             distance * Vector3.forward + height * Vector3.up;
                 
        ;
    }
    #endregion
}
