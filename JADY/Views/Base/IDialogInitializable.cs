namespace JADY.Views.Base;

public interface IDialogInitializable<T>
{
    void Initialize(T data);
}