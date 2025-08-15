using UnityEngine;
using UnityEngine.UI;

public class PlayerChargeBar : MonoBehaviour
{
    public GameObject chargeBar;
    public Image filling;
    Vector3 offset = new Vector3(0, 2f, 0);


    private void Awake()
    {
        if(chargeBar != null)
        {
            chargeBar.SetActive(false);
        }
    }

    public void ShowChargeBar()
    {
        Vector3 pos = transform.position + offset;
        if (chargeBar != null)
        {
            chargeBar.transform.position = pos;
        }
        chargeBar?.SetActive(true);
    }

    public void HideChargeBar()
    {
        chargeBar?.SetActive(false);
    }

    public void UpdateChargeBar(float chargeAmount)
    {
        if (filling != null)
        {
            filling.fillAmount = chargeAmount;
            filling.color = Color.Lerp(Color.red, Color.green, chargeAmount);
        }
    }
}
