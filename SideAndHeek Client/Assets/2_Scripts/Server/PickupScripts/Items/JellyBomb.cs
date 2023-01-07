using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyBomb : SpawnableObject
{
    public float explosionRadius;
    public AnimationCurve explosionSpeed;
    public float throwForceMultiplier;
    public int lifeDuration;

    private Rigidbody rigidbody;
    private float currentExplosionSize;
    private float startExplosionSize;
    private bool isExploding = false;
    private List<Player> trappedPlayers = new List<Player>();

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        startExplosionSize = transform.localScale.x;
        currentExplosionSize = startExplosionSize;
    }

    private void FixedUpdate()
    {
        if (isExploding)
        {
            if (currentExplosionSize > explosionRadius)
            {
                float curveValue = (explosionRadius - startExplosionSize) / currentExplosionSize;
                transform.localScale = Vector3.one * currentExplosionSize * Time.deltaTime;
            }
            else
            {
                currentExplosionSize = startExplosionSize;
                isExploding = false;

                //
            }
        }
    }

    public void Init(ushort _creatorId, int _code, Vector3 throwDirection, float throwForce)
    {
        Init(_creatorId, _code);

        rigidbody.AddForce(throwDirection * throwForce * throwForceMultiplier);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isExploding)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.useGravity = false;

            isExploding = true;
        }
           
        if (other.CompareTag("BodyCollider"))
        {
            Player player = other.GetComponentInParent<Player>();
            //trap them


            trappedPlayers.Add(player);
        }
    }

    IEnumerator StartLifetimeCoundown()
    {
        int currentLifetimeCount = lifeDuration;
        while(currentLifetimeCount > 0)
        {
            yield return new WaitForSeconds(1);

            currentLifetimeCount--;
        }

        //destroy this;
    }
}
