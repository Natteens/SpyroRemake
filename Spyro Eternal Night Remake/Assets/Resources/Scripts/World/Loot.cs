using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Health,
    Mana,
    FuryEnergy
}

public class Loot : MonoBehaviour
{
    public StatType statType;
    public float recoveryAmount = 20f;
    public float followPlayerRadius = 5f; 
    public float followSpeed = 5f; 

    private Transform player;
    private bool isFollowingPlayer = false;
    private Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= followPlayerRadius)
            isFollowingPlayer = true;
        else
            isFollowingPlayer = false;


        if (isFollowingPlayer)
        {
            transform.position += directionToPlayer.normalized * followSpeed * Time.deltaTime;
            if (distanceToPlayer <= 0.5f)
            {
                CollectLoot();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isFollowingPlayer = true;
        }
        else if (other.CompareTag("Ground"))
        {
            rb.isKinematic = true;
        }
    }

    private void CollectLoot()
    {
        Status playerStatus = player.GetComponent<Status>();

        if (playerStatus != null)
        {
            switch (statType)
            {
                case StatType.Health:
                    playerStatus.RechargeHealth(recoveryAmount);
                    break;
                case StatType.Mana:
                    playerStatus.RechargeMana(recoveryAmount);
                    break;
                case StatType.FuryEnergy:
                    playerStatus.GainFuryEnergy(recoveryAmount);
                    break;
            }
            Destroy(gameObject);
        }
    }
}
