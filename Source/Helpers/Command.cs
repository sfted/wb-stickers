using System;
using System.Windows.Input;

namespace WbStickers.Source.Helpers;

public class Command : ICommand
{
    public Command(Action execute) : this(execute, null) { }

    public Command(Action execute, Predicate<object?>? canExecute)
    {
        this.execute = execute ??
            throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    readonly Action execute;
    readonly Predicate<object?>? canExecute = null;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        canExecute == null || canExecute(parameter);

    public void Execute(object? parameter) =>
        execute?.Invoke();

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class Command<T> : ICommand
{
    public Command(Action<T?> execute)
        : this(execute, null) { }

    public Command(Action<T?> execute, Predicate<T?>? canExecute)
    {
        this.execute = execute ??
            throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    readonly Action<T?> execute;
    readonly Predicate<T?>? canExecute = null;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        canExecute == null || canExecute((T?)parameter);

    public void Execute(object? parameter) =>
        execute?.Invoke((T?)parameter);

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}