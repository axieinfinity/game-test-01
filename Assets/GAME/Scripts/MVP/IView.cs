public interface IView<in T> : IView where T : IModel
{
    void UpdateView(T model);
}

public interface IView
{
}