# Console progress

A console component of progress bar to let user know the status updated of the job.

In `Trivial.Console` [namespace](../) of `Trivial.Console.dll` [library](../../).

## Console progress component

You can output a progress bar with customized style to the standard output stream.

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
