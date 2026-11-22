
using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private TMP_Text counterText;

    public Rigidbody2D PlayerBody => playerBody;
    public int Coins { get; private set; }

    private void Start()
    {
        UpdateText();
    }

    public void AddCoin()
    {
        Coins++;
        UpdateText();
    }

    private void UpdateText()
    {
        if (counterText != null)
            counterText.text = $"Monedas: {Coins}";
    }
}
