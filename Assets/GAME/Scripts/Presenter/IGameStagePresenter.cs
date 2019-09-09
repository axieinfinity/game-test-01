using System;

public interface IGameStagePresenter : IPresenter
{
    IGameStagePresenter SetSpineData(SpineData[] spineData);

    IGameStagePresenter SetDataIds(string[] idArray,
        string playerDataId);

    IGameStagePresenter Init();
    void UpdateView();
    void InvokePlayerAttack();
    IGameStagePresenter SetConfig(GameConfig gameConfig);
    IObservable<bool> OnGameOverAsObservable();
    void Clear();
}