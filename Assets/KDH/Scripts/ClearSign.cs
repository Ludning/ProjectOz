using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClearSign : MonoBehaviour
{
    public Image image;
    public float fadeInDuration = 1f;
    public float displayDuration = 2f;
    public float fadeOutDuration = 1f;

    public void Start()
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }
    private void OnTriggerEnter(Collider other)
    {
        // DOTween을 사용하여 알파 값을 1로 변경하며 페이드인 효과를 줍니다.
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, fadeInDuration)) // 페이드인
                .AppendInterval(displayDuration) // 일정 시간 동안 유지
                .Append(image.DOFade(0, fadeOutDuration)); // 페이드아웃
    }
}
