using System.Collections;
using System.Collections.Generic;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using System.Threading.Tasks;

public class Economy
{
    private string flopCurrencyId = "FLOP";
    private string beanBillsCurrencyId = "BEAN_BILL";

    Dictionary<string, PlayerBalance> currencyBalances = new Dictionary<string, PlayerBalance>();

    public async Task Init()
    {
        GetBalancesResult getBalancesResult = await EconomyService.Instance.PlayerBalances.GetBalancesAsync(new GetBalancesOptions { ItemsPerFetch = 2 });
        foreach (PlayerBalance balance in getBalancesResult.Balances)
        {
            currencyBalances.Add(balance.CurrencyId, balance);
        }

        UpdateUI(true);
    }

    public async void DepositFlops(int value)
    {
        currencyBalances[flopCurrencyId] = await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(currencyBalances[flopCurrencyId].CurrencyId, value);

        UpdateUI();
    }

    public async void WithdrawFlops(int value)
    {
        currencyBalances[flopCurrencyId] = await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync(currencyBalances[flopCurrencyId].CurrencyId, value);

        UpdateUI();
    }

    public async void DepositBeanBills(int value)
    {
        currencyBalances[beanBillsCurrencyId] = await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(currencyBalances[beanBillsCurrencyId].CurrencyId, value);

        UpdateUI();
    }

    public async void WithdrawBeanBills(int value)
    {
        currencyBalances[beanBillsCurrencyId] = await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync(currencyBalances[beanBillsCurrencyId].CurrencyId, value);

        UpdateUI();
    }

    public void UpdateUI(bool animateOverride = false)
    {
        UIManager.instance.currencyUI.SetCurrencyValues((int)currencyBalances[flopCurrencyId].Balance, (int)currencyBalances[beanBillsCurrencyId].Balance, animateOverride);
    }
}
