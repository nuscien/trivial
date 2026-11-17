# Observable properties

在 `Trivial.Data` [命名空间](../)中。

<!-- Start -->

## Base observable properties

可以继承 `BaseObservableProperties` 类来实现具有可观察属性的模型。以下是一个示例。

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
