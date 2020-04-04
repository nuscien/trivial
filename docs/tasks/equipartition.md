# Equipartition Task

You can create a set of equipartition task fragment to ask the distributed client to process.

In `Trivial.Tasks` [namespace](./README) of `Trivial.dll` library.

## Task and fragments

You can simply create a specific number of equipartition task fragment with a job identifier by following code.

```csharp
// Create a task of a specific job which we call a-job here;
// And then separate it as 5 fragments.
var task = new EquipartitionTask("a-job", 5);
```

Then you can pick a fragment to process. This should be called by a client through web API or by another thread.

```csharp
var fragment = task.Pick();
```

The state of the fragment picked will be set as `Working` now from original `Pending`. The fragment will be `null` if no more fragment can be assigned.

You can set the state as `Success` or `Fatal` after the task fragment processes succeeded.

```csharp
task.UpdateFragment(fragment, EquipartitionTask.FragmentStates.Success);
```

The state flow is following here.

- `Pending` -> `Working` or `Ignored`;
- `Working` -> `Success`, `Failure`, `Fatal` or `Ignored`;
- `Failure` -> `Retrying`;
- `Retrying` -> `Success`, `Failure`, `Fatal` or `Ignored`;
- `Success`, `Fatal` and `Ignore` cannot change to any others.

## Task container

You can create an equipartition task container to manage the creation and assignment of the task with group supports.

```csharp
var taskController = new EquipartitionTaskContainer();
var task1 = taskController.Create("service-1", "job-1", 5);
var task2 = taskController.Create("service-1", "job-2", 1);
var task3 = taskController.Create("service-2", "job-3", 8);
```

And you can add the events after task creating and fragment state changing to sync to database or other place.
