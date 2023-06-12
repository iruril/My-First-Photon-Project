using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    public float speed = 10.0f;
    private void Update()
    {
        Rotation();
    }

    void Rotation()
    {
        transform.Rotate(Vector3.down * speed * Time.deltaTime);
    }
}
