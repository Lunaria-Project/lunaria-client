using Lunaria;
using UnityEngine;

public class PowderPortalScoreText : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Animator _animator;

    private static readonly int PlusTriggerHash = Animator.StringToHash("plus");
    private static readonly int MinusTriggerHash = Animator.StringToHash("minus");
    private static readonly int FeverTriggerHash = Animator.StringToHash("fever");

    private readonly Color _minusColor = Color.red;
    private readonly Color _plusColor = Color.green;

    public void Play(int score, bool isFever)
    {
        _text.SetText(score > 0 ? $"+{score}" : score.ToString());
        _text.color = score > 0 ? _plusColor : _minusColor;

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        var triggerHash = score > 0 ? (isFever ? FeverTriggerHash : PlusTriggerHash) : MinusTriggerHash;
        _animator.SetTrigger(triggerHash);
    }
}
