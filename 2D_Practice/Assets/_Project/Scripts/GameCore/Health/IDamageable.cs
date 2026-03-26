namespace GameCore.Health
{
    public interface IDamageable
    {
        void TakeDamage(float value);
        void TakeHeal(float value);

    }
}
