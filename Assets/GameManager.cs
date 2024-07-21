using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class GameManager : SingleTonMono<GameManager>
{
    float enemies_count;

    [SerializeField] Image image;

    [SerializeField] float fadeInDuration = 1f;
    [SerializeField] float displayDuration = 2f;
    [SerializeField] float fadeOutDuration = 1f;


    // Start is called before the first frame update
    private void Start()
    {
        Enemy[] enemy = FindObjectsOfType<Enemy>();
        enemies_count += (from Enemy enm in enemy select enm).Count();

        if (enemy.Length > 0)
        {
            foreach (var enm in enemy)
            {
                if(enm.isActiveAndEnabled)
                    enm.GetCombat().OnDead += OnEnemyDead;
            }
        }

        PlayerStat player = FindObjectOfType<PlayerStat>();

        player.PlayerCombat.OnDead += OnPlayerDead;

    }

    void OnEnemyDead()
    {
        enemies_count--;
        if( enemies_count == 0 )
        {
            Sprite cider = ResourceManager.Instance.LoadResource<Sprite>("Game_Clear_UI");
            OnEnableEndingCredit(cider);
        }
    }
    void OnPlayerDead()
    {
        Sprite cider = ResourceManager.Instance.LoadResource<Sprite>("Game_Over_UI");
        OnEnableEndingCredit(cider);
    }

    void OnEnableEndingCredit(Sprite sprite)
    {
        image.sprite = sprite;
        // DOTween을 사용하여 알파 값을 1로 변경하며 페이드인 효과를 줍니다.
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, fadeInDuration)) // 페이드인
                .AppendInterval(displayDuration) // 일정 시간 동안 유지
                .Append(image.DOFade(0, fadeOutDuration)); // 페이드아웃

        float AllTimeSeconds = fadeInDuration + displayDuration + fadeOutDuration + 0.5f;
        Invoke("ResetGame", AllTimeSeconds);
    }
    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        OzMagicManager.Instance.OnSceneLoaded();
    }
}
