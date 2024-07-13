using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image foregroundImage;
    [SerializeField] private float updateSpeedSeconds = 0.5f;

    private Camera cameraObject;

    private void Start() {
        GetComponentInParent<Health>().OnHealthPctChanged += HandleHealthChanged;
        cameraObject = Camera.main;
    }

    private void HandleHealthChanged(float pct) {
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator ChangeToPct(float pct) {

        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds) {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = pct;
    }

    private void LateUpdate() {
        if (cameraObject.IsDestroyed()) {
            cameraObject = Camera.main;
        }
        transform.LookAt(cameraObject.transform);
        transform.Rotate(0, 180, 0);
    }
}