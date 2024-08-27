using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Metrics;

namespace MechEngineerSaver;

public interface IOrderedElement { public int OrderNumber { get; set; } }
public interface IIndexedElement { public int Index { get; set; } }

public class DispatchedCollection<T> : ObservableCollection<T>
{
    public DispatchedCollection() : base() { }
    public DispatchedCollection(IEnumerable<T> collection) : base(collection ??= new List<T>()) { }
    public DispatchedCollection(List<T> list) : base(list) { }

    private object _syncKey = new();
    private void Locked(Action command)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            lock (_syncKey) command();
        else App.Current.Dispatcher.Invoke(() => { lock (_syncKey) command(); });
    }
    private void Locked<E>(E item, Action<E> command)
    {
        if (item == null) return;
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                                                   lock (_syncKey) command(item);
        else App.Current.Dispatcher.Invoke(() => { lock (_syncKey) command(item); });
    }
    private bool Locked<E>(E item, Func<E, bool> command)
    {
        if (item == null) return false;
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                                                          lock (_syncKey) return command(item);
        else return App.Current.Dispatcher.Invoke(() => { lock (_syncKey) return command(item); });
    }
    private void Locked<E>(IEnumerable<E> items, Action<E> command)
    {
        if (items == null) return;
        if (items == this) throw new Exception("Самовызов");
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                                                   lock (_syncKey) items.ForEach(i => command(i));
        else App.Current.Dispatcher.Invoke(() => { lock (_syncKey) items.ForEach(i => command(i)); });
    }
    private void Locked<E>(IEnumerable<E>? items, Action<IEnumerable<E>?> command)
    {
        if (items == this) throw new Exception("Самовызов");
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                                                   lock (_syncKey) command(items);
        else App.Current.Dispatcher.Invoke(() => { lock (_syncKey) command(items); });
    }
    private bool Locked<E>(IEnumerable<E> items, Func<E, bool> command)
    {
        if (items == null) return false;
        if (items == this) throw new Exception("Самовызов");
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                                                          lock (_syncKey) return items.Select(i => command(i)).All(i => i == true);
        else return App.Current.Dispatcher.Invoke(() => { lock (_syncKey) return items.Select(i => command(i)).All(i => i == true); });
    }


    protected void Base_Clear() => Locked(base.Clear);
    protected void Base_Add(T item) => Locked(item, base.Add);
    protected void Base_Insert(int index, T item) => Locked(item, item => base.Insert(index, item));
    protected bool Base_Remove(T item) => Locked(item, base.Remove);

    public new virtual void Clear() => Base_Clear();
    public new virtual void Add(T item) => Base_Add(item);
    public new virtual void Insert(int index, T item) => Base_Insert(index, item);
    public new virtual bool Remove(T item) => Base_Remove(item);

    protected void Base_AddRange(IEnumerable<T> items) => Locked(items, base.Add);
    protected bool Base_RemoveRange(IEnumerable<T> items) => Locked(items, base.Remove);
    protected void Base_Reset(IEnumerable<T>? items) => Locked(items, items =>
    {
        base.Clear();
        items?.ForEach(i => base.Add(i));
    });

    public virtual void AddRange(IEnumerable<T> items) => Base_AddRange(items);
    public virtual bool RemoveRange(IEnumerable<T> items) => Base_RemoveRange(items);
    public virtual void Reset(IEnumerable<T>? items) => Base_Reset(items);


    protected void Base_Move(int oldIndex, int newIndex)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            lock (_syncKey) base.Move(oldIndex, newIndex);
        else App.Current.Dispatcher.Invoke(() => { lock (_syncKey) base.Move(oldIndex, newIndex); });
    }
    protected void Base_RemoveAt(int index)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            lock (_syncKey) base.RemoveAt(index);
        else App.Current.Dispatcher.Invoke(() => { lock (_syncKey) base.RemoveAt(index); });
    }


    public new virtual void Move(int oldIndex, int newIndex) => Base_Move(oldIndex, newIndex);
    public new virtual void RemoveAt(int index) => Base_RemoveAt(index);
}

public class OrderedDispatchedCollection<T> : DispatchedCollection<T> where T : IOrderedElement
{
    public OrderedDispatchedCollection() : base() { }
    public OrderedDispatchedCollection(IEnumerable<T> collection, bool Ordered = false) : base(collection)
    { if (!Ordered) AssignmentOfOrders(); }
    public OrderedDispatchedCollection(List<T> list, bool Ordered = false) : base(list)
    { if (!Ordered) AssignmentOfOrders(); }

    protected void AssignmentOfOrders()
    {
        for (int i = 0; i < Count; i++)
            this[i].OrderNumber = i + 1;
    }
    private void AssignmentOfOrders(int startOrder, int stopOrder)
    {
        if (stopOrder < startOrder)
        {
            for (int i = stopOrder; i <= startOrder; i++)
                this[i].OrderNumber = i + 1;
        }
        else
        {
            for (int i = startOrder; i <= stopOrder; i++)
                this[i].OrderNumber = i + 1;
        }
    }
    private void AssignmentOfOrders(int startOrder)
    {
        for (int i = startOrder; i < Count; i++)
            this[i].OrderNumber = i + 1;
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldStartingIndex != -1 && e.OldItems != null && e.NewStartingIndex != -1 && e.NewItems != null)
            AssignmentOfOrders(e.OldStartingIndex, e.NewStartingIndex + e.NewItems.Count - 1);
        else if (e.OldStartingIndex != -1 && e.OldItems != null)
        {
            AssignmentOfOrders(e.OldStartingIndex);
        }
        else if (e.NewStartingIndex != -1 && e.NewItems != null)
        {
            AssignmentOfOrders(e.NewStartingIndex + e.NewItems.Count - 1);
        }

        base.OnCollectionChanged(e);
    }
}

public class IndexedDispatchedCollection<T> : DispatchedCollection<T> where T : IIndexedElement
{
    public IndexedDispatchedCollection() : base() { }
    public IndexedDispatchedCollection(IEnumerable<T> collection, bool Indexed = false) : base(collection)
    { if (!Indexed) AssignmentOfOrders(); }
    public IndexedDispatchedCollection(List<T> list, bool Indexed = false) : base(list)
    { if (!Indexed) AssignmentOfOrders(); }

    protected void AssignmentOfOrders()
    {
        for (int i = 0; i < Count; i++)
            this[i].Index = i;
    }
    private void AssignmentOfOrders(int startOrder, int stopOrder)
    {
        if (stopOrder < startOrder)
        {
            for (int i = stopOrder; i <= startOrder; i++)
                this[i].Index = i;
        }
        else
        {
            for (int i = startOrder; i <= stopOrder; i++)
                this[i].Index = i;
        }
    }
    private void AssignmentOfOrders(int startOrder)
    {
        for (int i = startOrder; i < Count; i++)
            this[i].Index = i;
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldStartingIndex != -1 && e.OldItems != null && e.NewStartingIndex != -1 && e.NewItems != null)
            AssignmentOfOrders(e.OldStartingIndex, e.NewStartingIndex + e.NewItems.Count - 1);
        else if (e.OldStartingIndex != -1 && e.OldItems != null)
        {
            AssignmentOfOrders(e.OldStartingIndex);
        }
        else if (e.NewStartingIndex != -1 && e.NewItems != null)
        {
            AssignmentOfOrders(e.NewStartingIndex + e.NewItems.Count - 1);
        }

        base.OnCollectionChanged(e);
    }
}