using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CristalType
{
    Health,
    Mana,
    Fury
}

public class Cristais : MonoBehaviour
{
    public CristalType Type;
    public short maxHealth = 150;
    public short currentHealth;

    public GameObject HealthDropPrefab;
    public GameObject ManaDropPrefab;   
    public GameObject FuryDropPrefab;   

    public Transform spawnPoint;
    public byte dropAmount = 5;

    public float launchForce = 5.0f;
    public float launchAngle = 45.0f;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(short damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            BreakCrystal();
        }

        if (currentHealth <= 100)
        {
            CreateSmallCrystals();
        }
        else if (currentHealth <= 50)
        {
            CreateSmallCrystals();
        }
    }

    private void BreakCrystal()
    {
        CreateSmallCrystals();
        Destroy(gameObject);
    }

    private void CreateSmallCrystals()
    {
        for (int i = 0; i < dropAmount; i++)
        {
            float randomAngle = Random.Range(-launchAngle, launchAngle);
            float radianAngle = randomAngle * Mathf.Deg2Rad;
            Vector3 launchDirection = new Vector3(Mathf.Sin(radianAngle), 0, Mathf.Cos(radianAngle));
            Vector3 launchForceVector = launchDirection * launchForce;

            GameObject selectedDropPrefab = null;

           
            switch (Type)
            {
                case CristalType.Health:
                    selectedDropPrefab = HealthDropPrefab;
                    break;
                case CristalType.Mana:
                    selectedDropPrefab = ManaDropPrefab;
                    break;
                case CristalType.Fury:
                    selectedDropPrefab = FuryDropPrefab;
                    break;
            }

            // Cria o drop usando o prefab selecionado
            GameObject smallCrystal = Instantiate(selectedDropPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody rb = smallCrystal.GetComponent<Rigidbody>();
            rb.AddForce(launchForceVector, ForceMode.Impulse);
        }
    }
}
