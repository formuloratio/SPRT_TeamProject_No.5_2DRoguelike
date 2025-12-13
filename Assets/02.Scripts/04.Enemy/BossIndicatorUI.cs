using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image arrowImage;

    private Enemy targetBoss;
    private Transform playerTransform;
    private Camera camera;

    [Header("보스 위치 표시 세팅")]
    [SerializeField] private float borderWidth;
    [SerializeField] private Vector2 size = new Vector2(100f, 100f);

    private CanvasScaler canvasScaler;

    public void SetUp(Enemy boss, Transform player)
    {
        targetBoss = boss;
        playerTransform = player;
        camera = Camera.main;

        Canvas ParentCanvas = GetComponentInParent<Canvas>();

        if (ParentCanvas != null)
        {
            canvasScaler = ParentCanvas.GetComponent<CanvasScaler>();
        }

        targetBoss.OnThisBossDied += HandleTargetBossDied;

        gameObject.GetComponentInChildren<RectTransform>().sizeDelta = size;

        arrowImage.gameObject.SetActive(false);
    }

    private void HandleTargetBossDied(Enemy diedBoss)
    {
        if (targetBoss == diedBoss)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (targetBoss == null || playerTransform == null || camera == null || canvasScaler == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 viewportPoint = camera.WorldToViewportPoint(targetBoss.transform.position);
        bool isOffScreen = viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1;

        arrowImage.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {
            Vector2 directionToBoss = (targetBoss.transform.position - playerTransform.position).normalized;
            float angle = Mathf.Atan2(directionToBoss.y, directionToBoss.x) * Mathf.Rad2Deg;
            arrowImage.rectTransform.localEulerAngles = new Vector3(0, 0, angle - 90);

            Vector2 referenceResolution = canvasScaler.referenceResolution;

            Vector2 targetScreenPos = camera.WorldToScreenPoint(targetBoss.transform.position);
            Vector2 targetScreenPosScaled = new Vector2(
                targetScreenPos.x / Screen.width * referenceResolution.x,
                targetScreenPos.y / Screen.height * referenceResolution.y
            );

            Vector2 canvasCenter = referenceResolution / 2f;
            Vector2 fromCenterToTarget = targetScreenPosScaled - canvasCenter;

            float maxX = (referenceResolution.x / 2f) - borderWidth - (size.x / 2f);
            float maxY = (referenceResolution.y / 2f) - borderWidth - (size.y / 2f);

            float clampedX = Mathf.Clamp(fromCenterToTarget.x, -maxX, maxX);
            float clampedY = Mathf.Clamp(fromCenterToTarget.y, -maxY, maxY);

            if (clampedX == maxX || clampedX == -maxX)
            {
                clampedY = fromCenterToTarget.y / Mathf.Abs(fromCenterToTarget.x) * Mathf.Abs(clampedX);
                clampedY = Mathf.Clamp(clampedY, -maxY, maxY);
            }
            else if (clampedY == maxY | clampedY == -maxY)
            {
                clampedX = fromCenterToTarget.x / Mathf.Abs(fromCenterToTarget.y) * Mathf.Abs(clampedY);
                clampedX = Mathf.Clamp(clampedX, -maxX, maxX);
            }
            arrowImage.rectTransform.localPosition = new Vector3(clampedX, clampedY, 0);
        }
    }

    private void OnDestroy()
    {
        if (targetBoss != null)
        {
            targetBoss.OnThisBossDied -= HandleTargetBossDied;
        }
    }
}
