using MunicipalServicesApp.Model;

public class RequestPriorityHeap
{
    // underlying list representing a binary min-heap (smallest Priority at root)
    private readonly List<ServiceRequest> items = new List<ServiceRequest>();

    // --------------------------------------------- index helpers -----------------------------------------------------//
    // parent / left / right index calculations for a binary heap stored in array
    private int Parent(int i) => (i - 1) / 2;
    private int Left(int i) => 2 * i + 1;
    private int Right(int i) => 2 * i + 2;

    // number of items currently in the heap
    public int Count => items.Count;

    // -------------------------------------------------- public API ----------------------------------------------------//

    // Insert a request into the heap (min-heap by Priority).
    // If req is null, do nothing.
    public void Insert(ServiceRequest req)
    {
        if (req == null) return;

        // Add new item to the end then bubble it up to maintain heap property
        items.Add(req);
        int i = items.Count - 1;

        while (i > 0 && items[Parent(i)].Priority > items[i].Priority)
        {
            // swap with parent
            var tmp = items[i];
            items[i] = items[Parent(i)];
            items[Parent(i)] = tmp;
            i = Parent(i);
        }
    }

    // Remove and return the smallest-priority request (root).
    // Returns null if heap is empty.
    public ServiceRequest ExtractMin()
    {
        if (items.Count == 0) return null;
        if (items.Count == 1)
        {
            var only = items[0];
            items.RemoveAt(0);
            return only;
        }

        // move last item to root, remove last, then heapify down
        var root = items[0];
        items[0] = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        Heapify(0);
        return root;
    }

    // Peek at the root (smallest priority) without removing it
    public ServiceRequest Peek() => items.Count > 0 ? items[0] : null;

    // ----------------------------------------------------------- internal helpers ----------------------------------------------------------//

    // Heapify down from index i to restore min-heap property
    private void Heapify(int i)
    {
        int smallest = i;
        int l = Left(i);
        int r = Right(i);

        if (l < items.Count && items[l].Priority < items[smallest].Priority) smallest = l;
        if (r < items.Count && items[r].Priority < items[smallest].Priority) smallest = r;

        if (smallest != i)
        {
            var tmp = items[i];
            items[i] = items[smallest];
            items[smallest] = tmp;
            Heapify(smallest);
        }
    }

    // Clear all items from heap
    public void Clear() => items.Clear();
}