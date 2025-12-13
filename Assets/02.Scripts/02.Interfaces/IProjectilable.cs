using UnityEngine;

public interface IProjectilable
{
    /// <summary>
    /// 투사체를 초기화
    /// </summary>
    /// <param name="target">투사체를 발사할 방향</param>
    /// <param name="projectileDamage">투사체 데미지</param>
    /// <param name="projectilePrefab">투사체 프리팹</param>
    void Initialize(Transform target, float projectileDamage, GameObject projectilePrefab);

    /// <summary>
    /// 투사체의 이동속도 설정
    /// </summary>
    /// <param name="speed">이동속도</param>
    void SetSpeed(float speed);

    /// <summary>
    /// 투사체의 수명 (씬에 얼만큼의 시간동안 존재할 것인지) 설정
    /// </summary>
    /// <param name="lifeTime">투사체 수명</param>
    void SetLifeTime(float lifeTime);

    /// <summary>
    /// 오브젝트 풀 매니저 설정 (프리팹 반환용)
    /// </summary>
    /// <param name="objectPoolManager"></param>
    void SetObjectPoolManager(EnemyObjectPoolManager objectPoolManager);
}
