using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ViewBase<TViewModel, TMessage> : MonoBehaviour where TViewModel : ViewModelBase<TMessage>, new() where TMessage : MessageBase
{
    protected TViewModel _vm;
    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new TViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.RegisterEventsOnEnable();
        }
    }

    private void OnDisable()
    {
        if (_vm != null)
        {
            _vm.UnRegisterEventsOnDisable();
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }
    }
    
    //상속받아 구현해야함, UI의 값을 변경하는 코드
    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }
}