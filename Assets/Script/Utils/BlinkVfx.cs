using System.Collections;
using UnityEngine;

public class BlinkVfx : MonoBehaviour
{
    [SerializeField] float duration = 0.2f;
    [SerializeField] float interval = .05f;

    readonly float baseMagnitude = 0.1f;

    private bool isPlaying = false;

    [SerializeField] private Renderer _renderer;

    private Coroutine _blinkCoroutine;

    public void Play()
    {
        Blink();
    }

    public BlinkVfx Set(Renderer renderer, float interval = .05f)
    {
        _renderer = renderer;
        this.interval = interval;
        return this;
    }

    private void Blink()
    {
        if (isPlaying)
        {
            return;
        }
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        _blinkCoroutine = StartCoroutine(BlinkEffectCoroutine());
    }

    public IEnumerator BlinkEffectCoroutine()
    {
        isPlaying = true;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            _renderer.enabled = false;
            yield return new WaitForSeconds(interval);
            elapsed += interval;
            _renderer.enabled = true;
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
        isPlaying = false;
    }

    public void ResetEffect()
    {
        StopCoroutine(_blinkCoroutine);
        _renderer.enabled = true;
        isPlaying = false;
    }
}
