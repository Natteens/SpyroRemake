using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Inimigo", menuName = "Inimigo/Criar novo Inimigo")]
public class EnemyData : ScriptableObject
{
    public float vidaMax;

    public float speed;

    public short Attack;

    public short attackRange;

    public byte raySpacing;

    public byte rayCheck;

    public LayerMask groundLayer;

    public short DetectRay;
    public LayerMask whatIsPlayer;


}
