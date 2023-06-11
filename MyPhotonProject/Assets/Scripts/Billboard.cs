using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        this.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }
}
