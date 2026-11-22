
using UnityEngine;

public class PlayerKeyHolder : MonoBehaviour
{
    public bool HasKey { get; private set; }

    private FloatingKey2D carriedKey;

    public bool AcquireKey(FloatingKey2D key)
    {
        if (HasKey || key == null)
            return false;

        HasKey = true;
        carriedKey = key;
        return true;
    }

    public bool UseKey()
    {
        if (!HasKey)
            return false;

        HasKey = false;

        if (carriedKey != null)
            Destroy(carriedKey.gameObject);

        carriedKey = null;
        return true;
    }
}
