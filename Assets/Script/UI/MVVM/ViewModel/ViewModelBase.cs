using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ViewModelBase<TMessage> where TMessage : MessageBase
{
    public virtual void RefreshViewModel()
    {
        //GameLogicManager.Inst.RefreshCharacterInfo(tempId, vm.OnRefreshViewModel);
    }
    public void RegisterEventsOnEnable()
    {
        MessageManager.Instance.RegisterCallback<TMessage>(OnResponseMessage);
    }
    public void UnRegisterEventsOnDisable()
    {
        MessageManager.Instance.UnRegisterCallback<TMessage>();
    }
    protected virtual void OnResponseMessage(TMessage message)
    {
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
