using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public TMP_Text flopCurrencyText;
    public TMP_Text beanBillsCurrencyText;

    public float animationSpeed;

    private float currentFlops;
    private float currentBeanBills;

    private int targetFlops;
    private int targetBeanBills;

    private bool animateFlops = false;
    private bool animateBeanBills = false;

    private void Update()
    {
        if (animateFlops)
        {
            currentFlops = Mathf.Lerp(currentFlops, targetFlops, Time.deltaTime * animationSpeed);
            SetFlopCurrencyValue(Mathf.RoundToInt(currentFlops));

            if (currentFlops == targetFlops)
            {
                animateFlops = false;
            }
        }

        if (animateBeanBills)
        {
            currentBeanBills = Mathf.Lerp(currentBeanBills, targetBeanBills, Time.deltaTime * animationSpeed);
            SetBeanBillsCurrencyValue(Mathf.RoundToInt(currentBeanBills));

            if (currentBeanBills == targetBeanBills)
            {
                animateBeanBills = false;
            }
        }
    }

    public void SetCurrencyValues(int flopBalance, int beanBillsBalance, bool animateOverride = false)
    {
        if (flopBalance != targetFlops)
        {
            targetFlops = Mathf.Clamp(flopBalance, 0, int.MaxValue);

            if (!animateOverride)
            {
                animateFlops = true;
            } else
            {
                SetFlopCurrencyValue(targetFlops);
            }
        }

        if (beanBillsBalance != targetBeanBills)
        {
            targetBeanBills = Mathf.Clamp(beanBillsBalance, 0, int.MaxValue);

            if (!animateOverride)
            {
                animateBeanBills = true;
            }
            else
            {
                SetBeanBillsCurrencyValue(targetBeanBills);
            }
        }
    }

    private void SetFlopCurrencyValue(int flopBalance)
    {
        flopCurrencyText.text = flopBalance.ToString();
    }

    private void SetBeanBillsCurrencyValue(int beanBillsBalance)
    {
        beanBillsCurrencyText.text = beanBillsBalance.ToString();
    }

    public void OnFlopsCurrencyButtonPressed()
    {
        UnityAuthentication.instance.economy.DepositFlops(100);
    }

    public void OnBeanBillsCurrencyButtonPressed()
    {
        UnityAuthentication.instance.economy.DepositBeanBills(100);
    }
}
