using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // 플레이어
    public float smoothSpeed = 5f;
    public Vector3 offset;

    [Header("Map Object (Floor )")]
    public GameObject mapObject;

    private Vector2 minPos;
    private Vector2 maxPos;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        if (mapObject == null)
        {
            Debug.LogWarning("CameraFollow: mapObject를 할당하세요");
            return;
        }

        // 🔥 Renderer만 사용 → 어떤 맵이든 자동 지원
        Renderer renderer = mapObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("mapObject에 Renderer가 없습니다.");
            return;
        }

        Bounds b = renderer.bounds;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        minPos = new Vector2(b.min.x + camWidth, b.min.y + camHeight);
        maxPos = new Vector2(b.max.x - camWidth, b.max.y - camHeight);

        Debug.Log($"Camera Clamp 설정됨: min={minPos}, max={maxPos}");
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        float clampX = Mathf.Clamp(smoothed.x, minPos.x, maxPos.x);
        float clampY = Mathf.Clamp(smoothed.y, minPos.y, maxPos.y);

        transform.position = new Vector3(clampX, clampY, transform.position.z);
    }
}