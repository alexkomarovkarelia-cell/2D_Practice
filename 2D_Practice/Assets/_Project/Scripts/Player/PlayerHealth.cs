using GameCore.Health;
using System.Collections;
using UnityEngine;

public class PlayerHealth : ObjectHealth
{
    [SerializeField] private float _regenerationDelay = 5f;
    [SerializeField] private float _regenerationValue = 1f;

    private Coroutine _regenerationCoroutine;

    private void Start()
    {
        _regenerationCoroutine = StartCoroutine(Regeneration());
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth <= 0f)
        {
            Debug.Log("Игрок умер");

            if (_regenerationCoroutine != null)
            {
                StopCoroutine(_regenerationCoroutine);
                _regenerationCoroutine = null;
            }
        }
    }

    private IEnumerator Regeneration()
    {
        while (CurrentHealth > 0f)
        {
            yield return new WaitForSeconds(_regenerationDelay);
            TakeHeal(_regenerationValue);
        }
    }
}