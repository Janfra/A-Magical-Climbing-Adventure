using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Windball Logic", menuName = "Scriptable Objects/New Windball")]
public class SO_WindballLogic : SO_WindballStats
{
    [SerializeField]
    protected Windball spawnedBallPrefab;

    public void ThrowWindball(MagicCasting _caster, float _chargedValue)
    {
        Windball windball; 
        ObjectPooler.IPoolGetComponent getter = ObjectPooler.Instance.SpawnFromGetterPool(ObjectPooler.PoolObjName.Windball, new Vector3(_caster.MagicDirection.x, _caster.MagicDirection.y) + _caster.transform.position, Quaternion.identity);
        if(getter.TryGetComponent(out windball))
        {
            windball.Shoot(_caster.MagicDirection.normalized, this, _chargedValue);
        }
    }
}
