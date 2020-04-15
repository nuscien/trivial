# Console progress

A console component of progress bar to let user know the status updated of the job.

In `Trivial.Console` [namespace](../) of `Trivial.Console.dll` [library](../../).

## Progress control

We would like to do better than print `Loading...` when the program is proccessing a task for a long while.
Now we have the progress control `ProgressLine` to output a progress bar to the standard output stream.
And we can customize its style and other settings by using an instance of `ProgressLineOptions` class as a parameter.

```csharp
var Main(string[] args)
{
    // Define an options that you can custom the style.
    var progressOptions = new ProgressLineOptions();

    // Ouput the component in console and get the progress instance to update.
    var progress = LineUtilities.WriteLine("Processing", progressOptions);

    // A time-consuming work here.
    for (var i = 0; i <= 50; i++)
    {
        await Task.Delay(10);

        // And report the progress updated.
        progress.Report(0.02 * i);
    }
}
```
