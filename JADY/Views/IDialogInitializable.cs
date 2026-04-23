namespace JADY.Views;

public interface IDialogInitializable<T>
{
    void Initialize(T data);
}