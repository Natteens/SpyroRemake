using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Inimigo", menuName = "Inimigo/Criar novo Inimigo")]
public class EnemyData : ScriptableObject
{
    public float vidaMax;

    public float speed;

    public short Attack;

    [Header("Visao")]

    public float rotationSpeed = 5f;

    public float viewAngle = 60f;

    public float rayHeight = 1.5f;

    public float viewDistance = 5f;

    public int rayCount = 5;

    public short attackRange;

    public byte rayCheck;

    public LayerMask groundLayer;
    

    public LayerMask whatIsPlayer;

    public bool PodeAndar = false;
    
}
