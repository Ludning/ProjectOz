using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private PlayerStat _playerStat;
    private void Start()
    {
        _playerStat = FindAnyObjectByType<PlayerStat>();

        _playerStat.PlayerCombat.OnDead += OnDead;
    }

    private void OnDead()
    {
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);
        _playerStat.Respawn();
    }

}
