# 分片任务

在 `Trivial.Tasks` [命名空间](../)中。

<!-- Start -->

可以创建一组分片任务片段，以请求分布式客户端进行处理。

## 任务和片段

可以通过以下代码使用作业标识符简单地创建特定数量的分片任务片段。

```csharp
// Create a task of a specific job which we call a-job here;
// And then separate it as 5 fragments.
var task = new EquipartitionTask("a-job", 5);
```

然后可以选择一个片段进行处理。这应该由客户端通过 web API 调用或由另一个线程调用。

```csharp
var fragment = task.Pick();
```

所选片段的状态将从原始的 `Pending` 设置为 `Working`。如果没有更多片段可以分配，片段将为 `null`。

在任务片段处理成功后，可以将状态设置为 `Success` 或 `Fatal`。

```csharp
task.UpdateFragment(fragment, EquipartitionTask.FragmentStates.Success);
```

状态流如下所示。

- `Pending` -> `Working` 或 `Ignored`;
- `Working` -> `Success`, `Failure`, `Fatal` 或 `Ignored`;
- `Failure` -> `Retrying`;
- `Retrying` -> `Success`, `Failure`, `Fatal` 或 `Ignored`;
- `Success`, `Fatal` 和 `Ignore` 不能更改为其他状态。

## 任务容器

可以创建一个分片任务容器来管理任务的创建和分配，并支持分组。

```csharp
var taskController = new EquipartitionTaskContainer();
var task1 = taskController.Create("service-1", "job-1", 5);
var task2 = taskController.Create("service-1", "job-2", 1);
var task3 = taskController.Create("service-2", "job-3", 8);
```

可以在任务创建和片段状态更改后添加事件，以同步到数据库或其他地方。
