using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArea : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] platformsToActivate;

    private bool hasActivatedPlatforms = false;

    void Update()
    {
        if (!hasActivatedPlatforms && AllEnemiesDefeated())
        {
            ActivatePlatforms();
        }
    }

    bool AllEnemiesDefeated()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeSelf)
            {              
                return false;
            }
        }
        return true;
    }

    void ActivatePlatforms()
    {
        hasActivatedPlatforms = true;

        foreach (GameObject platform in platformsToActivate)
        {
            if (platform != null)
            {
                platform.SetActive(true);
            }
        }
    }
}
