using UnityEngine;

public interface IBossAttackable
{
    void BossAttack(Enemy enemy, Transform target);

    bool IsActive { get; }

    void CancelPattern();

}
