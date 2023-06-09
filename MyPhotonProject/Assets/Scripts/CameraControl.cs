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


    //Late 를 입력하면 모든 스크립트의 Update 를 완료한 뒤에 해당 내용을 처리한다는 뜻이다.
    //(Late를 입력하지 않으면 특정PC 에서는 끊기는 현상 발생)
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
