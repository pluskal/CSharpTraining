# Async Programming : Patterns for Asynchronous MVVM Applications: Commands

[Stephen Cleary](https://msdn.microsoft.com/en-us/magazine/mt149362?author=Stephen+Cleary) | April 2014 | [Get the Code](https://msdn.microsoft.com/en-us/magazine/msdnmag0414)

This is the second article in a series on combining async and await with the established Model-View-ViewModel (MVVM) pattern. Last time, I showed how to data bind to an asynchronous operation, and I developed a key type called NotifyTaskCompletion<TResult> that acted like a data binding-friendly Task<TResult> (see [msdn.microsoft.com/magazine/dn605875](http://msdn.microsoft.com/magazine/dn605875)). Now I’ll turn to ICommand, a .NET interface used by MVVM applications to define a user operation (which is often data bound to a button), and I’ll consider the implications of making an asynchronous ICommand.

The patterns here may not fit every scenario perfectly, so feel free to tune them to your needs. In fact, this entire article is presented as a series of improvements on an asynchronous command type. At the end of these iterations, you’ll end up with an application like what’s shown in **Figure 1**. This is similar to the application developed in my last article, but this time I provide the user with an actual command to execute. When the user clicks the Go button, the URL is read from the textbox and the application will count the number of bytes at that URL (after an artificial delay). While the operation is in progress, the user may not start another one, but he can cancel the operation.

![An Application That Can Execute One Command](https://msdn.microsoft.com/dynimg/IC716164.png "An Application That Can Execute One Command")  
![An Application That Can Execute One Command](https://msdn.microsoft.com/dynimg/IC716163.png "An Application That Can Execute One Command")  
![An Application That Can Execute One Command](https://msdn.microsoft.com/dynimg/IC716162.png "An Application That Can Execute One Command")  
![An Application That Can Execute One Command](https://msdn.microsoft.com/dynimg/IC716161.png "An Application That Can Execute One Command")  
![An Application That Can Execute One Command](https://msdn.microsoft.com/dynimg/IC716160.png "An Application That Can Execute One Command")  
**Figure 1 An Application That Can Execute One Command**

I’ll then show how a very similar approach can be used to create any number of operations. **Figure 2** illustrates the application modified so the Go button represents adding an operation to a collection of operations.

![An Application Executing Multiple Commands](https://msdn.microsoft.com/dynimg/IC716159.png "An Application Executing Multiple Commands")  
**Figure 2 An Application Executing Multiple Commands**

There are a couple of simplifications I’m going to make during the development of this application, to keep the focus on asynchronous commands instead of implementation details. First, I won’t use command execution parameters. I’ve hardly ever needed to use parameters in real-world apps; but if you need them, the patterns in this article can be easily extended to include them. Second, I don’t implement ICommand.CanExecuteChanged myself. A standard field-like event will leak memory on some MVVM platforms (see [bit.ly/1bROnVj](http://bit.ly/1bROnVj)). To keep the code simple, I use the Windows Presentation Foundation (WPF) built-in CommandManager to implement CanExecuteChanged.

I’m also using a simplified “service layer,” which for now is just a single static method, as shown in **Figure 3**. It’s essentially the same service as in my last article, but extended to support cancellation. The next article will deal with proper asynchronous service design, but for now this simplified service will do.

***Figure 3 The Service Layer***

```C#
public static class MyService
{
  // bit.ly/1fCnbJ2
  public static async Task<span style="color:Blue;"><</span><span style="color:#A31515;">int</span><span style="color:Blue;">></span> DownloadAndCountBytesAsync(string url,
    CancellationToken token = new CancellationToken())
  {
    await Task.Delay(TimeSpan.FromSeconds(3), token).ConfigureAwait(false);
    var client = new HttpClient();
    using (var response = await client.GetAsync(url, token).ConfigureAwait(false))
    {
      var data = await
        response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
      return data.Length;
    }
  }
}
```

## Asynchronous Commands

Before getting started, take a quick look at the ICommand interface:


```C#
public interface ICommand
{
  event EventHandler CanExecuteChanged;
  bool CanExecute(object parameter);
  void Execute(object parameter);
}
```

Ignore CanExecuteChanged and the parameters, and think for a bit about how an asynchronous command would work with this interface. The CanExecute method must be synchronous; the only member that can be asynchronous is Execute. The Execute method was designed for synchronous implementations, so it returns void. As I mentioned in a previous article, “Best Practices in Asynchronous Programming” ([msdn.microsoft.com/magazine/jj991977](http://msdn.microsoft.com/magazine/jj991977)), async void methods should be avoided unless they’re event handlers (or the logical equiv­alent of event handlers). Implementations of ICommand.Execute are logically event handlers and, thus, may be async void.

However, it’s best to minimize the code within an async void method and expose an async Task method instead that contains the actual logic. This practice makes the code more testable. With this in mind, I propose the following as an asynchronous command interface, and the code in **Figure 4** as the base class:

```C#
public interface IAsyncCommand : ICommand
{
  Task ExecuteAsync(object parameter);
}
```

***Figure 4 Base Type for Asynchronous Commands***

```C#
public abstract class AsyncCommandBase : IAsyncCommand
{
  public abstract bool CanExecute(object parameter);
  public abstract Task ExecuteAsync(object parameter);
  public async void Execute(object parameter)
  {
    await ExecuteAsync(parameter);
  }
  public event EventHandler CanExecuteChanged
  {
    add { CommandManager.RequerySuggested += value; }
    remove { CommandManager.RequerySuggested -= value; }
  }
  protected void RaiseCanExecuteChanged()
  {
    CommandManager.InvalidateRequerySuggested();
  }
}
```

The base class takes care of two things: It punts the CanExecuteChanged implementation off to the CommandManager class; and it implements the async void ICommand.Execute method by calling the IAsyncCommand.ExecuteAsync method. It awaits the result to ensure that any exceptions in the asynchronous command logic will be properly raised to the UI thread’s main loop.

This is a fair amount of complexity, but each of these types has a purpose. IAsyncCommand can be used for any asynchronous ICommand implementation, and is intended to be exposed from ViewModels and consumed by the View and by unit tests. AsyncCommandBase handles some of the common boilerplate code common to all asynchronous ICommands.

With this groundwork in place, I’m ready to start developing an effective asynchronous command. The standard delegate type for a synchronous operation without a return value is Action. The asynchronous equivalent is Func<Task>. **Figure 5** shows my first iteration of a delegate-based AsyncCommand.

***Figure 5 The First Attempt at an Asynchronous Command***

```C#
public class AsyncCommand : AsyncCommandBase
{
  private readonly Func<span style="color:Blue;"><</span><span style="color:#A31515;">Task</span><span style="color:Blue;">></span> _command;
  public AsyncCommand(Func<span style="color:Blue;"><</span><span style="color:#A31515;">Task</span><span style="color:Blue;">></span> command)
  {
    _command = command;
  }
  public override bool CanExecute(object parameter)
  {
    return true;
  }
  public override Task ExecuteAsync(object parameter)
  {
    return _command();
  }
}
```

At this point, the UI has only a textbox for the URL, a button to start the HTTP request and a label for the results. The XAML and the essential parts of the ViewModel are simple. Here’s Main­Window.xaml (skipping the positioning attributes such as Margin):

```XML
<Grid>
  <TextBox Text="{Binding Url}" />
  <Button Command="{Binding CountUrlBytesCommand}" 
      Content="Go" />
  <TextBlock Text="{Binding ByteCount}" />
</Grid>
```

MainWindowViewModel.cs is shown in **Figure 6**.

```C#
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
  public MainWindowViewModel()
  {
    Url = "http://www.example.com/";
    CountUrlBytesCommand = new AsyncCommand(async () =>
    {
      ByteCount = await MyService.DownloadAndCountBytesAsync(Url);
    });
  }
  public string Url { get; set; } // Raises PropertyChanged
  public IAsyncCommand CountUrlBytesCommand { get; private set; }
  public int ByteCount { get; private set; } // Raises PropertyChanged
}
```

If you execute the application (AsyncCommands1 in the sample code download), you’ll notice four cases of inelegant behavior. First, the label always shows a result, even before the button is clicked. Second, there’s no busy indicator after you click the button to indicate the operation is in progress. Third, if the HTTP request faults, the exception is passed to the UI main loop, causing an application crash. Fourth, if the user makes several requests, she can’t distinguish the results; it’s possible for the results of an earlier request to overwrite the results of a later request due to varying server response times.

This is quite a slew of problems! But before I iterate the design, consider for a moment the kinds of issues raised. When a UI becomes asynchronous, it forces you to think about additional states in your UI. I recommend you ask yourself at least these questions:

1.  How will the UI display errors? (I hope your synchronous UI already has an answer for this one!)
2.  How should the UI look while the operation is in progress? (For example, will it provide immediate feedback via busy indicators?)
3.  How is the user restricted while the operation is in progress? (Are buttons disabled, for example?)
4.  Does the user have any additional commands available while the operation is in progress? (For instance, can he cancel the operation?)
5.  If the user can start multiple operations, how does the UI provide completion or error details for each one? (For example, will the UI use a “command queue” style or notification popups?)


## Handling Asynchronous Command Completion via Data Binding

Most of the problems in the first Async­Command iteration relate to how the results are handled. What’s really needed is some kind of type that would wrap a Task<T> and provide some data-binding capabilities so the application can respond more elegantly. As it happens, the NotifyTaskCompletion<T> type developed in my last article fits these needs almost perfectly. I’m going to add one member to this type that simplifies some of the Async­Command logic: a TaskCompletion property that represents the operation completing but doesn’t propagate exceptions (or return a result). Here are the modifications to NotifyTaskCompletion<T>:

```C#
public NotifyTaskCompletion(Task<TResult> task)
{
  Task = task;
  if (!task.IsCompleted)
    TaskCompletion = WatchTaskAsync(task);
}
public Task TaskCompletion { get; private set; }
```

The next iteration of AsyncCommand uses NotifyTaskCompletion to represent the actual operation. By doing so, the XAML can data bind directly to the result and error message of that operation, and it can also use data binding to display an appropriate message while the operation is in progress. The new AsyncCommand now has a property that represents the actual operation, as shown in **Figure 7**.

**Figure 7 The Second Attempt at an Asynchronous Command**

```C#
public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
{
  private readonly Func<Task<TResult>> _command;
  private NotifyTaskCompletion<TResult> _execution;
  public AsyncCommand(Func<Task<TResult>> command)
  {
    _command = command;
  }
  public override bool CanExecute(object parameter)
  {
    return true;
  }
  public override Task ExecuteAsync(object parameter)
  {
    Execution = new NotifyTaskCompletion<TResult>(_command());
    return Execution.TaskCompletion;
  }
  // Raises PropertyChanged
  public NotifyTaskCompletion<TResult> Execution { get; private set; }
}
```

Note that AsyncCommand.ExecuteAsync is using TaskCompletion and not Task. I don’t want to propagate exceptions to the UI main loop (which would happen if it awaited the Task property); instead, I return TaskCompletion and handle exceptions by data binding. I also added a simple NullToVisibilityConverter to the project so that the busy indicator, results and error message are all hidden until the button is clicked. **Figure 8** shows the updated ViewModel code.

**Figure 8 The Second MainWindowViewModel**

```C#
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
  public MainWindowViewModel()
  {
    Url = "http://www.example.com/";
    CountUrlBytesCommand = new AsyncCommand<int>(() => 
      MyService.DownloadAndCountBytesAsync(Url));
  }
  // Raises PropertyChanged
  public string Url { get; set; }
  public IAsyncCommand CountUrlBytesCommand { get; private set; }
}
```

And the new XAML code is shown in **Figure 9**.

**Figure 9 The Second MainWindow XAML**

```XML
<Grid>
  <TextBox Text="{Binding Url}" />
  <Button Command="{Binding CountUrlBytesCommand}" Content="Go" />
  <Grid Visibility="{Binding CountUrlBytesCommand.Execution,
    Converter={StaticResource NullToVisibilityConverter}}">
    <!--Busy indicator-->
    <Label Visibility="{Binding CountUrlBytesCommand.Execution.IsNotCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}"
      Content="Loading..." />
    <!--Results-->
    <Label Content="{Binding CountUrlBytesCommand.Execution.Result}"
      Visibility="{Binding CountUrlBytesCommand.Execution.IsSuccessfullyCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}" />
    <!--Error details-->
    <Label Content="{Binding CountUrlBytesCommand.Execution.ErrorMessage}"
      Visibility="{Binding CountUrlBytesCommand.Execution.IsFaulted,
      Converter={StaticResource BooleanToVisibilityConverter}}" Foreground="Red" />
  </Grid>
</Grid>
```

The code now matches the AsyncCommands2 project in the sample code. This code takes care of all the concerns I mentioned with the original solution: labels are hidden until the first operation starts; there’s an immediate busy indicator providing feedback to the user; exceptions are captured and update the UI via data binding; multiple requests no longer interfere with each other. Each request creates a new NotifyTaskCompletion wrapper, which has its own independent Result and other properties. NotifyTaskCompletion acts as a data-bindable abstraction of an asynchronous operation. This allows multiple requests, with the UI always binding to the latest request. However, in many real-world scenarios, the appropriate solution is to disable multiple requests. That is, you want the command to return false from CanExecute while there’s an operation in progress. This is easy enough to do with a small modification to AsyncCommand, as shown in **Figure 10**.

**Figure 9 The Second MainWindow XAML**

```C#
<Grid>
  <TextBox Text="{Binding Url}" />
  <Button Command="{Binding CountUrlBytesCommand}" Content="Go" />
  <Grid Visibility="{Binding CountUrlBytesCommand.Execution,
    Converter={StaticResource NullToVisibilityConverter}}">
    <!--Busy indicator-->
    <Label Visibility="{Binding CountUrlBytesCommand.Execution.IsNotCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}"
      Content="Loading..." />
    <!--Results-->
    <Label Content="{Binding CountUrlBytesCommand.Execution.Result}"
      Visibility="{Binding CountUrlBytesCommand.Execution.IsSuccessfullyCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}" />
    <!--Error details-->
    <Label Content="{Binding CountUrlBytesCommand.Execution.ErrorMessage}"
      Visibility="{Binding CountUrlBytesCommand.Execution.IsFaulted,
      Converter={StaticResource BooleanToVisibilityConverter}}" Foreground="Red" />
  </Grid>
</Grid>
```

## Adding Cancellation

Many asynchronous operations can take varying amounts of time. For example, an HTTP request may normally respond very quickly, before the user can even respond. However, if the network is slow or the server is busy, that same HTTP request might cause a considerable delay. Part of designing an asynchronous UI is expecting and designing for this scenario. The current solution already has a busy indicator. When you design an asynchronous UI, you can also choose to give the user more options, and cancellation is a common choice.

Cancellation itself is always a synchronous operation—the act of requesting cancellation is immediate. The trickiest part of cancellation is when it can run; it should be able to execute only when there’s an asynchronous command in progress. The modifications to AsyncCommand in **Figure 11** provide a nested cancellation command and notify that cancellation command when the asynchronous command begins and ends.

**Figure 11 Adding Cancellation**

```C#
public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
{
  private readonly Func<CancellationToken, Task<TResult>> _command;
  private readonly CancelAsyncCommand _cancelCommand;
  private NotifyTaskCompletion<TResult> _execution;
  public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
  {
    _command = command;
    _cancelCommand = new CancelAsyncCommand();
  }
  public override async Task ExecuteAsync(object parameter)
  {
    _cancelCommand.NotifyCommandStarting();
    Execution = new NotifyTaskCompletion<TResult>(_command(_cancelCommand.Token));
    RaiseCanExecuteChanged();
    await Execution.TaskCompletion;
    _cancelCommand.NotifyCommandFinished();
    RaiseCanExecuteChanged();
  }
  public ICommand CancelCommand
  {
    get { return _cancelCommand; }
  }
  private sealed class CancelAsyncCommand : ICommand
  {
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private bool _commandExecuting;
    public CancellationToken Token { get { return _cts.Token; } }
    public void NotifyCommandStarting()
    {
      _commandExecuting = true;
      if (!_cts.IsCancellationRequested)
        return;
      _cts = new CancellationTokenSource();
      RaiseCanExecuteChanged();
    }
    public void NotifyCommandFinished()
    {
      _commandExecuting = false;
      RaiseCanExecuteChanged();
    }
    bool ICommand.CanExecute(object parameter)
    {
      return _commandExecuting && !_cts.IsCancellationRequested;
    }
    void ICommand.Execute(object parameter)
    {
      _cts.Cancel();
      RaiseCanExecuteChanged();
    }
  }
}
```

Adding a Cancel button (and a canceled label) to the UI is straightforward, as **Figure 12** shows.

**Figure 12 Adding a Cancel Button**

```XML
<Grid>
  <TextBox Text="{Binding Url}" />
  <Button Command="{Binding CountUrlBytesCommand}" Content="Go" />
  <Button Command="{Binding CountUrlBytesCommand.CancelCommand}" Content="Cancel" />
  <Grid Visibility="{Binding CountUrlBytesCommand.Execution,
    Converter={StaticResource NullToVisibilityConverter}}">
    <!--Busy indicator-->
    <Label Content="Loading..."
      Visibility="{Binding CountUrlBytesCommand.Execution.IsNotCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}" />
    <!--Results-->
    <Label Content="{Binding CountUrlBytesCommand.Execution.Result}"
      Visibility="{Binding CountUrlBytesCommand.Execution.IsSuccessfullyCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}" />
    <!--Error details-->
    <Label Content="{Binding CountUrlBytesCommand.Execution.ErrorMessage}"
      Visibility="{Binding CountUrlBytesCommand.Execution.IsFaulted,
      Converter={StaticResource BooleanToVisibilityConverter}}" Foreground="Red" />
    <!--Canceled-->
    <Label Content="Canceled"
      Visibility="{Binding CountUrlBytesCommand.Execution.IsCanceled,
      Converter={StaticResource BooleanToVisibilityConverter}}" Foreground="Blue" />
  </Grid>
</Grid>
```

Now, if you execute the application (AsyncCommands4 in the sample code), you’ll find the cancel button is initially disabled. It’s enabled when you click the Go button and remains enabled until the operation completes (whether successfully, faulted or canceled). You now have an arguably complete UI for an asynchronous operation.


## A Simple Work Queue

Up to this point, I’ve been focusing on a UI for just one operation at a time. This is all that’s necessary in many situations, but sometimes you need the ability to start multiple asynchronous operations. In my opinion, as a community we haven’t come up with a really good UX for handling multiple asynchronous operations. Two common approaches are using a work queue or a notification system, neither of which is ideal.

A work queue displays all asynchronous operations in a collection; this gives the user maximum visibility and control, but is usually too complex for the typical end user to cope with. A notification system hides the operations while they’re running, and will pop up if any of them fault (and possibly if they complete successfully). A notification system is more user-friendly, but it doesn’t provide the full visibility and power of the work queue (for example, it’s difficult to work cancellation into a notification-based system). I have yet to discover an ideal UX for multiple asynchronous operations.

That said, the sample code at this point can be extended to support a multiple-operation scenario without too much trouble. In the existing code, the Go button and the Cancel button are both conceptually related to a single asynchronous operation. The new UI will change the Go button to mean “start a new asynchronous operation and add it to the list of operations.” What this means is that the Go button is now actually synchronous. I added a simple (synchronous) DelegateCommand to the solution, and now the ViewModel and XAML can be updated, as **Figure 13** and **Figure 14** show.

**Figure 13 ViewModel for Multiple Commands**

```C#
public sealed class CountUrlBytesViewModel
{
  public CountUrlBytesViewModel(MainWindowViewModel parent, string url,
    IAsyncCommand command)
  {
    LoadingMessage = "Loading (" + url + ")...";
    Command = command;
    RemoveCommand = new DelegateCommand(() => parent.Operations.Remove(this));
  }
  public string LoadingMessage { get; private set; }
  public IAsyncCommand Command { get; private set; }
  public ICommand RemoveCommand { get; private set; }
}
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
  public MainWindowViewModel()
  {
    Url = "http://www.example.com/";
    Operations = new ObservableCollection<CountUrlBytesViewModel>();
    CountUrlBytesCommand = new DelegateCommand(() =>
    {
      var countBytes = new AsyncCommand<int>(token =>
        MyService.DownloadAndCountBytesAsync(
        Url, token));
      countBytes.Execute(null);
      Operations.Add(new CountUrlBytesViewModel(this, Url, countBytes));
    });
  }
  public string Url { get; set; } // Raises PropertyChanged
  public ObservableCollection<CountUrlBytesViewModel> Operations
    { get; private set; }
  public ICommand CountUrlBytesCommand { get; private set; }
}
```

**Figure 14 XAML for Multiple Commands**

```XML
<Grid>
  <TextBox Text="{Binding Url}" />
  <Button Command="{Binding CountUrlBytesCommand}" Content="Go" />
  <ItemsControl ItemsSource="{Binding Operations}">
    <ItemsControl.ItemTemplate>
      <DataTemplate>
        <Grid>
          <!--Busy indicator-->
          <Label Content="{Binding LoadingMessage}"
            Visibility="{Binding Command.Execution.IsNotCompleted,
            Converter={StaticResource BooleanToVisibilityConverter}}" />
          <!--Results-->
          <Label Content="{Binding Command.Execution.Result}"
            Visibility="{Binding Command.Execution.IsSuccessfullyCompleted,
            Converter={StaticResource BooleanToVisibilityConverter}}" />
          <!--Error details-->
          <Label Content="{Binding Command.Execution.ErrorMessage}"
            Visibility="{Binding Command.Execution.IsFaulted,
            Converter={StaticResource BooleanToVisibilityConverter}}"
            Foreground="Red" />
          <!--Canceled-->
          <Label Content="Canceled"
            Visibility="{Binding Command.Execution.IsCanceled,
            Converter={StaticResource BooleanToVisibilityConverter}}"
            Foreground="Blue" />
          <Button Command="{Binding Command.CancelCommand}" Content="Cancel" />
          <Button Command="{Binding RemoveCommand}" Content="X" />
        </Grid>
      </DataTemplate>
    </ItemsControl.ItemTemplate>
  </ItemsControl>
</Grid>
```


This code is equivalent to the AsyncCommandsWithQueue project in the sample code. When the user clicks the Go button, a new AsyncCommand is created and wrapped into a child ViewModel (CountUrlBytesViewModel). This child ViewModel instance is then added to the list of operations. Everything associated with that particular operation (the various labels and the Cancel button) is displayed in a data template for the work queue. I also added a simple button “X” that will remove the item from the queue.

This is a very basic work queue, and I made some assumptions about the design. For example, when an operation is removed from the queue, it isn’t automatically canceled. When you start working with multiple asynchronous operations, I recommend you ask yourself at least these additional questions:

1.  How does the user know which notification or work item is for which operation? (For example, the busy indicator in this work queue sample contains the URL it’s downloading).
2.  Does the user need to know every result? (For example, it may be acceptable to notify the user only of errors, or to automatically remove successful operations from the work queue).

## Wrapping Up

There isn’t a universal solution for an asynchronous command that fits everyone’s needs—yet. The developer community is still exploring asynchronous UI patterns. My goal in this article is to show how to think about asynchronous commands in the context of an MVVM application, especially considering UX issues that must be addressed when the UI becomes asynchronous. But keep in mind the patterns in this article and sample code are just patterns, and should be adapted to the needs of the application.

In particular, there isn’t a perfect story regarding multiple asynchronous operations. There are drawbacks to both work queues and notifications, and it seems to me that a universal UX has yet to be developed. As more UIs become asynchronous, a lot more minds will be thinking about this problem, and a revolutionary breakthrough might be right around the corner. Give the problem some thought, dear reader. Perhaps you will be the discoverer of a new UX.

In the meantime, you still have to ship. In this article I started with the most basic of asynchronous ICommand implementations and gradually added features until I ended up with something fairly suitable for most modern applications. The result is also fully unit-testable; because the async void ICommand.Execute method only calls the Task-returning IAsyncCommand.ExecuteAsync method, you can use ExecuteAsync directly in your unit tests.

In my last article, I developed NotifyTaskCompletion<T>, a data-binding wrapper around Task<T>. In this one, I showed how to develop one kind of AsyncCommand<T>, an asynchronous implementation of ICommand. In my next article, I’ll address asynchronous services. Do bear in mind that asynchronous MVVM patterns are still quite new; don’t be afraid to deviate from them and innovate your own solutions.

# Acknowledgment

All credits go to **Stephen Cleary**, author of the original [article](https://msdn.microsoft.com/en-us/magazine/dn630647.aspx)