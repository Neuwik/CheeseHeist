using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUser : MonoBehaviour
{
    private AAbility CurrentAbility;
    public GameObject AbilityPanel;
    public Image img_Icon;
    public TMP_Text txt_Name;
    public GameObject WheelCenter;
    public Canvas UICanvas;

    private bool isUsingAbility = false;

    private void Awake()
    {
        AbilityPanel.SetActive(false);
    }

    public bool SetAbility(AAbility NewAbility)
    {
        if (NewAbility == null)
        {
            return false;
        }

        if (CurrentAbility != null)
        {
            return false;
        }

        CurrentAbility = NewAbility;

        img_Icon.sprite = CurrentAbility.Icon;
        txt_Name.text = CurrentAbility.Name;
        AbilityPanel.SetActive(true);

        return true;
    }

    public void OnUseAbility()
    {
        if (!isUsingAbility &&CurrentAbility != null)
        {
            isUsingAbility = true;

            AAbility ability = CurrentAbility;
            CurrentAbility = null;

            if (ability.UseAbility(this))
            {
                AbilityPanel.SetActive(false);
            }
            else
            {
                CurrentAbility = ability;
            }

            isUsingAbility = false;
        }
    }
}
