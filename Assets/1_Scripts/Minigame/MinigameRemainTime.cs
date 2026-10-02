using Lunaria;
using UnityEngine;

public class MinigameRemainTime : MonoBehaviour
{
    private const float FullRotationAngle = -360f;
    private const int RemainTimeDigits = 2;

    [SerializeField] private Image _remainTimeImage;
    [SerializeField] private Text[] _remainTimeTexts;
    [SerializeField] private Transform _rotateTransform;

    public void SetRemainTime(float remainTime, float totalTime)
    {
        var progress = (totalTime - remainTime) / totalTime;
        _remainTimeImage.fillAmount = progress;
        _remainTimeTexts.SetTexts(Mathf.RoundToInt(remainTime).ToNDigits(RemainTimeDigits));
        _rotateTransform.localEulerAngles = new Vector3(0f, 0f, FullRotationAngle * progress);
    }
}
