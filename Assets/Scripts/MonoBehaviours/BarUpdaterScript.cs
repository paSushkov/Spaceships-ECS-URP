using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUpdaterScript : MonoBehaviour
{
    private bool allVarsAssigned = false;
    public GameManager manager;
    public Image filler;
    public Text text;
    [SerializeField]
    private BarType barType = BarType.HealthBar;

    // Start is called before the first frame update
    void Start()
    {
        allVarsAssigned = checkVarsAssignation();
        if (allVarsAssigned)
        {
            switch (barType)
            {
                case (BarType.HealthBar):
                    UpdateContent(manager.MaxPlayerHealth, manager.CurrentPlayerHealth);
                    manager.NotifyHealthChanges += UpdateContent;
                    break;
                case (BarType.ShieldBar):
                    UpdateContent(manager.MaxPlayerShield, manager.CurrentPlayerShield);
                    manager.NotifyShieldChanges += UpdateContent;
                    break;

            }
        }
    }

    private void OnDestroy()
    {
        switch (barType)
        {
            case (BarType.HealthBar):
                manager.NotifyHealthChanges -= UpdateContent;
                break;
            case (BarType.ShieldBar):
                manager.NotifyShieldChanges -= UpdateContent;
                break;
        }
    }

    private bool checkVarsAssignation()
    {
        if (manager != null && filler != null && text != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateContent(float maxAmount, float currentAmount)
    {
        filler.fillAmount = currentAmount / maxAmount;
        text.text = String.Format("{0:0}", currentAmount);
    }
}
public enum BarType { HealthBar, ShieldBar };