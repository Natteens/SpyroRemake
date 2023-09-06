using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersonagensDB 
{
    [SerializeField] short vidaMax; // Vida maxima de um personagem
    [SerializeField] short velocidade; // velocidade maxima de um personagem
    [SerializeField] short ataque; // dano de ataque de um personagem

    public short VidaMax
    {
        get { return vidaMax; }
    }
    public short Velocidade
    {
        get { return velocidade; }
    }
    public short Ataque
    {
        get { return ataque; }
    }
}
