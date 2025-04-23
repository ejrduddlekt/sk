using System;

public interface IRecordView<T>
{
    void Init(T data);
    event Action<T> OnClicked;
}