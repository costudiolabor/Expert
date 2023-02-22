using UnityEngine;

[System.Serializable]
public class ViewOperator<TPrefab> where TPrefab : AnimatedView {
    [SerializeField] private TPrefab prefab;
    [SerializeField] protected RectTransform viewParent;
    protected TPrefab view;

    public virtual void CreateView(){
        view = Object.Instantiate(prefab, viewParent);
        view.ForceClose();
        view.Close();
    }
}