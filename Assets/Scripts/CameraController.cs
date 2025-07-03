using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private RectTransform playableArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CenterCameraAroundBusstop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // centers camera, so playing fild is same size around busstop. takes shop window into consideration
    private void CenterCameraAroundBusstop()
    {
        Vector3 screenPos = playableArea.TransformPoint(playableArea.rect.center);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos) * -1;
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);

    }
}
