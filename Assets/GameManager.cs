using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : SingleTonMono<GameManager>
{
    float enemies_count;

    [SerializeField] float fadeInDuration = 1f;
    [SerializeField] float displayDuration = 2f;
    [SerializeField] float fadeOutDuration = 1f;

    private void Awake()
    {
        //image.sprite = ResourceManager.Instance.LoadResource<Sprite>("Game_Clear_UI");
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        Enemy[] enemy = FindObjectsOfType<Enemy>();
        enemies_count += (from Enemy enm in enemy select enm).Count();

        foreach(var enm in enemy)
        {
            enm.GetCombat().OnDead += OnEnemyDead;
        }
    }

    void OnEnemyDead()
    {
        enemies_count--;
        if( enemies_count == 0 )
        {
            OnEnableEndingCredit();
        }
    }
    void OnEnableEndingCredit()
    {
        //// DOTween을 사용하여 알파 값을 1로 변경하며 페이드인 효과를 줍니다.
        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(image.DOFade(1, fadeInDuration)) // 페이드인
        //        .AppendInterval(displayDuration) // 일정 시간 동안 유지
        //        .Append(image.DOFade(0, fadeOutDuration)); // 페이드아웃
    }
}
