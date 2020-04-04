# Observable properties

In `Trivial.Data` [namespace](./README) of `Trivial.dll` [library](../README).

## Base observable properties

You can inherit the `BaseObservableProperties` class to implement your model with observable properties. Following is a sample.

```csharp
    public class NameValueObservableProperties<T> : BaseObservableProperties
    {
        public string Id
        {
            get => GetCurrentProperty<string>();
            set => SetCurrentProperty(value);
        }
    }
```
