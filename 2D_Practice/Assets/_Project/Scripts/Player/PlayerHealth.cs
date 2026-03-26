using GameCore.Health;
using System.Collections;
using UnityEngine;

public class PlayerHealth: ObjectHealth

{
    private WaitForSeconds _regenerationInterval = new WaitForSeconds(5f);
    private float _regenerationValue = 1f;

    private void Start()
    {
        StartCoroutine(routine: Regeration());
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (CurrentHealth <= 0)
            Debug.Log("Игрок умер");
    }
    private IEnumerator Regeration ()
    {
        while(true)
        {
            TakeHeal (_regenerationValue);
            yield return _regenerationInterval;
        }
    }
}
