# Async Programming : Patterns for Asynchronous MVVM Applications: Data Binding

[Stephen Cleary](https://msdn.microsoft.com/en-us/magazine/mt149362?author=Stephen+Cleary) | March 2014

Asynchronous code using the async and await keywords is transforming the way programs are written, and with good reason. Although async and await can be useful for server software, most of the current focus is on applications that have a UI. For such applications, these keywords can yield a more responsive UI. However, it’s not immediately obvious how to use async and await with established patterns such as Model-View-ViewModel (MVVM). This article is the first in a short series that will consider patterns for combining async and await with MVVM.

To be clear, my first article on async, “Best Practices in Asynchronous Programming” ([msdn.microsoft.com/magazine/jj991977](http://msdn.microsoft.com/magazine/jj991977)), was relevant to all applications that use async/await, both client and server. This new series builds on the best practices in that article and introduces patterns specifically for client-side MVVM applications. These patterns are just patterns, however, and may not necessarily be the best solutions for a specific scenario. If you find a better way, let me know!

As of this writing, the async and await keywords are supported on a wide number of MVVM platforms: desktop (Windows Presentation Foundation [WPF] on the Microsoft .NET Framework 4 and higher), iOS/Android (Xamarin), Windows Store (Windows 8 and higher), Windows Phone (version 7.1 and higher), Silverlight (version 4 and higher), as well as Portable Class Libraries (PCLs) targeting any mix of these platforms (such as MvvmCross). The time is now ripe for “async MVVM” patterns to develop.

I’m assuming you’re somewhat familiar with async and await and quite familiar with MVVM. If that’s not the case, there are a number of helpful introductory materials available online. My blog ([bit.ly/19IkogW](http://bit.ly/19IkogW)) includes an async/await intro that lists additional resources at the end, and the MSDN documentation on async is quite good (search for “Task-based Asynchronous Programming”). For more information on MVVM, I recommend pretty much anything written by Josh Smith.

## A Simple Application

In this article, I’m going to build an incredibly simple application, as **Figure 1** shows. When the application loads, it starts an HTTP request and counts the number of bytes returned. The HTTP request may complete successfully or with an exception, and the application will update using data binding. The application is fully responsive at all times.

![The Sample Application](https://msdn.microsoft.com/dynimg/IC712702.png "The Sample Application")  
![The Sample Application](https://msdn.microsoft.com/dynimg/IC712701.png "The Sample Application")  
![The Sample Application](https://msdn.microsoft.com/dynimg/IC712700.png "The Sample Application")  
**Figure 1 The Sample Application**

First, though, I want to mention that I follow the MVVM pattern rather loosely in my own projects, sometimes using a proper domain Model, but more often using a set of services and data transfer objects (essentially a data access layer) instead of an actual Model. I’m also rather pragmatic when it comes to the View; I don’t shy away from a few lines of codebehind if the alternative is dozens of lines of code in supporting classes and XAML. So, when I talk about MVVM, understand that I’m not using any particular strict definition of the term.

One of the first things you have to consider when introducing async and await to the MVVM pattern is identifying which parts of your solution need the UI threading context. Windows platforms are serious about UI components being accessed only from the UI thread that owns them. Obviously, the view is entirely tied to the UI context. I also take the stand in my applications that anything linked to the view via data binding is tied to the UI context. Recent versions of WPF have loosened this restriction, allowing some sharing of data between the UI thread and background threads (for example, BindingOperations.EnableCollection­Synchronization). However, support for cross-thread data binding isn’t guaranteed on every MVVM platform (WPF, iOS/Android/Windows Phone, Windows Store), so in my own projects I just treat anything data-bound to the UI as having UI-thread affinity.

As a result, I always treat my ViewModels as though they’re tied to the UI context. In my applications, the ViewModel is more closely related to the View than the Model—and the ViewModel layer is essentially an API for the entire application. The View literally provides just the shell of UI elements in which the actual application exists. The ViewModel layer is conceptually a testable UI, complete with a UI thread affinity. If your Model is an actual domain model (not a data access layer) and there’s data binding between the Model and ViewModel, then the Model itself also has UI-thread affinity. Once you’ve identified which layers have UI affinity, you should be able to draw a mental line between the “UI-affine code” (View and ViewModel, and possibly the Model) and the “UI-agnostic code” (probably the Model and definitely all other layers, such as services and data access).

Furthermore, all code outside the View layer (that is, the ViewModel and Model layers, services, and so on) should not depend on any type tied to a specific UI platform. Any direct use of Dispatcher (WPF/Xamarin/Windows Phone/Silverlight), CoreDispatcher (Windows Store), or ISynchronizeInvoke (Windows Forms) is a bad idea. (SynchronizationContext is marginally better, but barely.) For example, there’s a lot of code on the Internet that does some asynchronous work and then uses Dispatcher to update the UI; a more portable and less cumbersome solution is to use await for asynchronous work and update the UI without using Dispatcher.

ViewModels are the most interesting layer because they have UI affinity but don’t depend on a specific UI context. In this series, I’ll combine async and MVVM in ways that avoid specific UI types while also following async best practices; this first article focuses on asynchronous data binding.

## Asynchronous Data-Bound Properties

The term “asynchronous property” is actually an oxymoron. Property getters should execute immediately and retrieve current values, not kick off background operations. This is likely one of the reasons the async keyword can’t be used on a property getter. If you find your design asking for an asynchronous property, consider some alternatives first. In particular, should the property actually be a method (or a command)? If the property getter needs to kick off a new asynchronous operation each time it’s accessed, that’s not a property at all. Asynchronous methods are straightforward, and I’ll cover asynchronous commands in another article.

In this article, I’m going to develop an asynchronous data-bound property; that is, a data-bound property that I update with the results of an async operation. One common scenario is when a ViewModel needs to retrieve data from some external source.

As I explained earlier, for my sample application, I’m going to define a service that counts the bytes in a Web page. To illustrate the responsiveness aspect of async/await, this service will also delay a few seconds. I’ll cover more realistic asynchronous services in a later article; for now, the “service” is just the single method shown in **Figure 2**.

**Figure 2 MyStaticService.cs**
```C#
using System;
using System.Net.Http;
using System.Threading.Tasks;
public static class MyStaticService
{
  public static async Task<int> CountBytesInUrlAsync(string url)
  {
    // Artificial delay to show responsiveness.
    await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
    // Download the actual data and count it.
    using (var client = new HttpClient())
    {
      var data = await client.GetByteArrayAsync(url).ConfigureAwait(false);
      return data.Length;
    }
  }
}
```

Note that this is considered a service, so it’s UI-agnostic. Because the service is UI-agnostic, it uses ConfigureAwait(false) every time it does an await (as discussed in my other article, “Best Practices in Asynchronous Programming”).

Let’s add a simple View and ViewModel that starts an HTTP request on startup. The example code uses WPF windows with the Views creating their ViewModels on construction. This is just for simplicity; the async principles and patterns discussed in this series of articles apply across all MVVM platforms, frameworks and libraries. The View for now will consist of a single main window with a single label. The XAML for the main View just binds to the UrlByteCount member:

```XML
<Window x:Class="MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Grid>
    <Label Content="{Binding UrlByteCount}"/>
  </Grid>
</Window>
```

The codebehind for the main window creates the ViewModel:

```C#
public partial class MainWindow
{
  public MainWindow()
  {
    DataContext = new BadMainViewModelA();
    InitializeComponent();
  }
}
```

## Common Mistakes

You might notice the ViewModel type is called BadMainViewModelA. This is because I’m going to first look at a couple of common mistakes relating to ViewModels. One common mistake is to synchronously block on the operation, like so:

```C#
public class BadMainViewModelA
{
  public BadMainViewModelA()
  {
    // BAD CODE!!!
    UrlByteCount =
      MyStaticService.CountBytesInUrlAsync("http://www.example.com").Result;
  }
  public int UrlByteCount { get; private set; }
}
```

This is a violation of the async guideline “async all the way,” but sometimes developers try this if they feel they’re out of options. If you execute that code, you’ll see it works, to a certain extent. Code that uses Task.Wait or Task<T>.Result instead of await is synchronously blocking on that operation.

There are a few problems with synchronous blocking. The most obvious is the code is now taking an asynchronous operation and blocking on it; by doing so, it loses all the benefits of asynchronicity. If you execute the current code, you’ll see the application does nothing for a few seconds, and then the UI window springs fully formed into view with its results already populated. The problem is the application is unresponsive, which is unacceptable for many modern applications. The example code has a deliberate delay to emphasize that unresponsiveness; in a real-world application, this problem might go unnoticed during development and show up only in “unusual” client scenarios (such as loss of network connectivity).

Another problem with synchronous blocking is more subtle: The code is more brittle. My example service uses ConfigureAwait(false) properly, just as a service should. However, this is easy to forget, especially if you (or your coworkers) don’t regularly use async. Consider what could happen over time as the service code is maintained. A maintenance developer might forget a ConfigureAwait, and at that point the blocking of the UI thread would become a deadlock of the UI thread. (This is described in more detail in my previous article on async best practices.)

OK, so you should use “async all the way.” However, many developers proceed to the second faulty approach, illustrated in **Figure 3**.

**Figure 3 BadMainViewModelB.cs**

```C#
using System.ComponentModel;
using System.Runtime.CompilerServices;
public sealed class BadMainViewModelB : INotifyPropertyChanged
{
  public BadMainViewModelB()
  {
    Initialize();
  }
  // BAD CODE!!!
  private async void Initialize()
  {
    UrlByteCount = await MyStaticService.CountBytesInUrlAsync(
      "http://www.example.com");
  }
  private int _urlByteCount;
  public int UrlByteCount
  {
    get { return _urlByteCount; }
    private set { _urlByteCount = value; OnPropertyChanged(); }
  }
  public event PropertyChangedEventHandler PropertyChanged;
  private void OnPropertyChanged([CallerMemberName] string propertyName = null)
  {
    PropertyChangedEventHandler handler = PropertyChanged;
    if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
  }
}
```

Again, if you execute this code, you’ll find that it works. The UI now shows immediately, with “0” in the label for a few seconds before it’s updated with the correct value. The UI is responsive, and everything seems fine. However, the problem in this case is handling errors. With an async void method, any errors raised by the asynchronous operation will crash the application by default. This is another situation that’s easy to miss during development and shows up only in “weird” conditions on client devices. Even changing the code in **Figure 3** from async void to async Task barely improves the application; all errors would be silently ignored, leaving the user wondering what happened. Neither method of handling errors is appropriate. And though it’s possible to deal with this by catching exceptions from the asynchronous operation and updating other data-bound properties, that would result in a lot of tedious code.

</div>

<div>

## A Better Approach

Ideally, what I really want is a type just like Task<T> with properties for getting results or error details. Unfortunately, Task<T> is not data-binding friendly for two reasons: it doesn’t implement INotify­PropertyChanged and its Result property is blocking. However, you can define a “Task watcher” of sorts, such as the type in **Figure 4**.

**Figure 4 NotifyTaskCompletion.cs**

```C#
using System;
using System.ComponentModel;
using System.Threading.Tasks;
public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
{
  public NotifyTaskCompletion(Task<TResult> task)
  {
    Task = task;
    if (!task.IsCompleted)
    {
      var _ = WatchTaskAsync(task);
    }
  }
  private async Task WatchTaskAsync(Task task)
  {
    try
    {
      await task;
    }
    catch
    {
    }
    var propertyChanged = PropertyChanged;
    if (propertyChanged == null)
        return;
    propertyChanged(this, new PropertyChangedEventArgs("Status"));
    propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
    propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
    if (task.IsCanceled)
    {
      propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
    }
    else if (task.IsFaulted)
    {
      propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
      propertyChanged(this, new PropertyChangedEventArgs("Exception"));
      propertyChanged(this,
        new PropertyChangedEventArgs("InnerException"));
      propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
    }
    else
    {
      propertyChanged(this,
        new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
      propertyChanged(this, new PropertyChangedEventArgs("Result"));
    }
  }
  public Task<TResult> Task { get; private set; }
  public TResult Result { get { return (Task.Status == TaskStatus.RanToCompletion) ?
    Task.Result : default(TResult); } }
  public TaskStatus Status { get { return Task.Status; } }
  public bool IsCompleted { get { return Task.IsCompleted; } }
  public bool IsNotCompleted { get { return !Task.IsCompleted; } }
  public bool IsSuccessfullyCompleted { get { return Task.Status ==
    TaskStatus.RanToCompletion; } }
  public bool IsCanceled { get { return Task.IsCanceled; } }
  public bool IsFaulted { get { return Task.IsFaulted; } }
  public AggregateException Exception { get { return Task.Exception; } }
  public Exception InnerException { get { return (Exception == null) ?
    null : Exception.InnerException; } }
  public string ErrorMessage { get { return (InnerException == null) ?
    null : InnerException.Message; } }
  public event PropertyChangedEventHandler PropertyChanged;
}
```

Let’s walk through the core method NotifyTaskCompletion<T>.WatchTaskAsync. This method takes a task representing the asynchronous operation, and (asynchronously) waits for it to complete. Note that the await does not use ConfigureAwait(false); I want to return to the UI context before raising the PropertyChanged notifications. This method violates a common coding guideline here: It has an empty general catch clause. In this case, though, that’s exactly what I want. I don’t want to propagate exceptions directly back to the main UI loop; I want to capture any exceptions and set properties so that the error handling is done via data binding. When the task completes, the type raises PropertyChanged notifications for all the appropriate properties.

An updated ViewModel using NotifyTaskCompletion<T> would look like this:

```C#
public class MainViewModel
{
  public MainViewModel()
  {
    UrlByteCount = new NotifyTaskCompletion<int>(
      MyStaticService.CountBytesInUrlAsync("http://www.example.com"));
  }
  public NotifyTaskCompletion<int> UrlByteCount { get; private set; }
}
```

This ViewModel will start the operation immediately and then create a data-bound “watcher” for the resulting task. The View data-binding code needs to be updated to bind explicitly to the result of the operation, like this:

```XML
<Window x:Class="MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Grid>
    <Label Content="{Binding UrlByteCount.Result}"/>
  </Grid>
</Window>
```

Note that the label content is data-bound to NotifyTask­Completion<T>.Result, not Task<T>.Result. NotifyTaskCompletion<T>.Result is data-binding friendly: It is not blocking, and it will notify the binding when the task completes. If you run the code now, you’ll find it behaves just like the previous example: The UI is responsive and loads immediately (displaying the default value of “0”) and then updates in a few seconds with the actual results.

The benefit of NotifyTaskCompletion<T> is it has many other properties as well, so you can use data binding to show busy indicators or error details. It isn’t difficult to use some of these convenience properties to create a busy indicator and error details completely in the View, such as the updated data-binding code in **Figure 5**.

**Figure 5 MainWindow.xaml**
```XML
<Window x:Class="MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Window.Resources>
    <BooleanToVisibilityConverter x:Key="BooleanToVisibilityConverter"/>
  </Window.Resources>
  <Grid>
    <!-- Busy indicator -->
    <Label Content="Loading..." Visibility="{Binding UrlByteCount.IsNotCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}"/>
    <!-- Results -->
    <Label Content="{Binding UrlByteCount.Result}" Visibility="{Binding
      UrlByteCount.IsSuccessfullyCompleted,
      Converter={StaticResource BooleanToVisibilityConverter}}"/>
    <!-- Error details -->
    <Label Content="{Binding UrlByteCount.ErrorMessage}" Background="Red"
      Visibility="{Binding UrlByteCount.IsFaulted,
      Converter={StaticResource BooleanToVisibilityConverter}}"/>
  </Grid>
</Window>
```

With this latest update, which changes only the View, the application displays “Loading…” for a few seconds (while remaining responsive) and then updates to either the results of the operation or to an error message displayed on a red background.

NotifyTaskCompletion<T> handles one use case: When you have an asynchronous operation and want to data bind the results. This is a common scenario when doing data lookups or loading during startup. However, it doesn’t help much when you have an actual command that’s asynchronous, for example, “save the current record.” (I’ll consider asynchronous commands in my next article.)

At first glance, it seems like it’s a lot more work to build an asynchronous UI, and that’s true to some extent. Proper use of the async and await keywords strongly encourages you to design a better UX. When you move to an asynchronous UI, you find you can no longer block the UI while an asynchronous operation is in progress. You must think about what the UI should look like during the loading process, and purposefully design for that state. This is more work, but it is work that should be done for most modern applications. And it’s one reason that newer platforms such as the Windows Store support only asynchronous APIs: to encourage developers to design a more responsive UX.

## Wrapping Up

When a code base is converted from synchronous to asynchronous, usually the service or data access components change first, and async grows from there toward the UI. Once you’ve done it a few times, translating a method from synchronous to asynchronous becomes fairly straightforward. I expect (and hope) that this translation will be automated by future tooling. However, when async hits the UI, that’s when real changes are necessary.

When the UI becomes asynchronous, you must address situations where your applications are unresponsive by enhancing their UI design. The end result is a more responsive, more modern application. “Fast and fluid,” if you will.

This article introduced a simple type that can be summed up as a Task<T> for data binding. Next time, I’ll look at asynchronous commands, and explore a concept that’s essentially an “ICommand for async.” Then, in the final article in the series, I’ll wrap up by considering asynchronous services. Keep in mind the community is still developing these patterns; feel free to adjust them for your particular needs.


# Acknowledgment

All credits go to **Stephen Cleary**, author of the original [article](https://msdn.microsoft.com/en-us/magazine/dn630647.aspx)