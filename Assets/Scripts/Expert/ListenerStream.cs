using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ListenerStream : ViewOperator<ListenerStreamView>
{
    [SerializeField] private ListenerStreamModel prefabListenerStreamModel;
    
    private ListenerStreamModel _listenerStreamModel;

    public void Initialize() {
        base.CreateView();
        _listenerStreamModel = Object.Instantiate(prefabListenerStreamModel);
        view.Open();
        
    }
}
