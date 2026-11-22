using UnityEngine;

public class Trampolin : Trap
{
    private void Awake()
    {
        
        damage = 0;
    }

    private void Reset()
    {
        // Valores por defecto al agregar el script
        damage = 0;
        bounceForce = 15f;
    }
}