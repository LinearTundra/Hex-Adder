using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Hex : MonoBehaviour
{
    [SerializeField]
    protected TMP_Text valueTextBox;
    public int Value { get; protected set; }

    protected SpriteRenderer _sprite;


    protected virtual void Awake()
    {
        Value = 0;
        _sprite = GetComponent<SpriteRenderer>();
        
        _sprite.enabled = false;
        valueTextBox.enabled = false;
        
        _sprite.color = Color.HSVToRGB(0, 0, .7f);
        UpdateTextBoxValue();
    }

    protected virtual void UpdateTextBoxValue()
    {
        if (Value <= 0) valueTextBox.text = "";
        else valueTextBox.text = Value.ToString();
    }

    protected virtual void SetHexColor(Color color)
    {
        _sprite.color = color;
    }
}
