using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Vector3 CamPosition = new Vector3(0f, 7f, 4.2f);

    [SerializeField]
    GameObject player = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        QuaterView();
    }

    void QuaterView()
    {
        transform.position = player.transform.position + CamPosition;
        transform.LookAt(player.transform.position);
    }
}
