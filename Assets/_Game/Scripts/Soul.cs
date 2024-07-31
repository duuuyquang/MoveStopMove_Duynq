using System.Collections;
using UnityEngine;

public class Soul : GameUnit
{
    private ColorType colorType;

    [SerializeField] ColorDataSO colorDataSO;
    [SerializeField] ParticleSystem partical;

    public void Init(Character target, ColorType type)
    {
        colorType = type;
        partical.startColor = colorDataSO.GetColor(colorType);
        TF.SetParent(target.TF);

        TF.localPosition = Cache.GetVector(Random.Range(-1f, 1.2f), Random.Range(-1f, 1.5f), Random.Range(-0.2f, -1.5f));
        TF.localScale = Vector3.one * 0.1f;
    }

    Coroutine trail; 
    public void MoveToTarget(Character target)
    {
        trail = StartCoroutine(IEMoveToTarget(target));
    }

    IEnumerator IEMoveToTarget(Character target)
    {
        Vector3 upperPos = TF.position + Vector3.up * 8f;
        while (Vector3.Distance(TF.position, upperPos) > 0.1f)
        {
            TF.position = Vector3.MoveTowards(TF.position, upperPos, 6f * Time.deltaTime);
            yield return Cache.GetWaitSecs(0.01f);
        }

        while (Vector3.Distance(TF.position, target.TF.position) > 0.1f)
        {
            //TF.position = Vector3.MoveTowards(TF.position, target.TF.position, 10f * Time.deltaTime);
            TF.position = Vector3.Lerp(TF.position, target.TF.position, 5f * Time.deltaTime);
            yield return Cache.GetWaitSecs(0.01f);
            if (target.IsStatus(StatusType.Dead))
            {
                StopCoroutine(trail);
                OnDespawn();
            }
        }

        if (Vector3.Distance(TF.position, target.TF.position) <= 0.1f)
        {
            OnDespawn();
            if(!target.IsStatus(StatusType.Dead))
            {
                target.CollectSoul(colorType);
            }
        }
    }

    public void OnDespawn()
    {
        SimplePool.Despawn(this);
    }

    public void OnStolen(Character opponent)
    {
        TF.SetParent(PoolControl.Instance.SoulPoolingTF);
        MoveToTarget(opponent);
    }
}
