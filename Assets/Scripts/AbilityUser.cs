using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUser : MonoBehaviour
{
    private Ability CurrentAbility;
    public GameObject AbilityPanel;
    public Image img_Icon;
    public TMP_Text txt_Name;
    public GameObject WheelCenter;

    private bool isUsingAbility = false;

    private void Awake()
    {
        AbilityPanel.SetActive(false);
    }

    public bool SetAbility(Ability NewAbility)
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

    private void OnUseAbility()
    {
        if (!isUsingAbility &&CurrentAbility != null)
        {
            isUsingAbility = true;

            Ability ability = CurrentAbility;
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
