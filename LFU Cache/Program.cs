using System;
using System.Collections.Generic;

namespace LFU_Cache
{
    class Program
    {
        static void Main(string[] args)
        {
            LFUCache cache = new LFUCache(2);
            cache.Put(1, 0);
            cache.Put(2, 2);
            Console.WriteLine(cache.Get(1));
            cache.Put(3, 3);
            Console.WriteLine(cache.Get(2));
            Console.WriteLine(cache.Get(3));
            cache.Put(4, 4);
            Console.WriteLine(cache.Get(1));
            Console.WriteLine(cache.Get(3));
            Console.WriteLine(cache.Get(4));
        }
    }

    public class LFUCache
    {
        readonly int capacity;
        // for each key and its value and frequency of usage
        Dictionary<int, (int value, int count, LinkedListNode<int> node)> keyCount;
        // group the frequency, same frequency keys will be together and linked list will have most recently used element at the top
        Dictionary<int, LinkedList<int>> countList;
        // when we have a tie, this prop will be used to get the least frequent group linkedlist and from that list will be removing the least recently used key
        int minCount;

        public LFUCache(int capacity)
        {
            this.capacity = capacity;
            minCount = 1;
            keyCount = new Dictionary<int, (int value, int count, LinkedListNode<int> node)>();
            countList = new Dictionary<int, LinkedList<int>>();
        }

        public int Get(int key)
        {
            if (!keyCount.ContainsKey(key))
            {
                return -1;
            }
            else
            {
                int value = keyCount[key].value;
                updateKey(key, value);
                return value;
            }
        }

        public void Put(int key, int value)
        {
            if (capacity < 1)
            {
                return;
            }

            if (!keyCount.ContainsKey(key))
            {
                if (keyCount.Count >= capacity)
                {
                    var minNode = countList[minCount].Last;
                    keyCount.Remove(minNode.Value);
                    countList[minCount].Remove(minNode);
                    if (countList[minCount].Count == 0)
                    {
                        countList.Remove(minCount);
                    }
                }
                int count = 1;
                minCount = 1;
                var node = new LinkedListNode<int>(key);
                var item = (value, count, node);
                keyCount[key] = item;
                if (!countList.ContainsKey(count))
                {
                    countList[count] = new LinkedList<int>();
                }
                countList[count].AddFirst(node);
            }
            else
            {
                updateKey(key, value);
            }
        }

        void updateKey(int key, int value)
        {
            var item = keyCount[key];
            int count = item.count;
            var node = item.node;
            countList[count].Remove(node);
            if (countList[count].Count == 0)
            {
                if (minCount == count)
                {
                    minCount = count + 1;
                }
                countList.Remove(count);
            }
            var nodeNew = new LinkedListNode<int>(key);
            var itemNew = (value, count + 1, nodeNew);
            keyCount[key] = itemNew;
            if (!countList.ContainsKey(count + 1))
            {
                countList[count + 1] = new LinkedList<int>();
            }
            countList[count + 1].AddFirst(nodeNew);
        }
    }

    /**
     * Your LFUCache object will be instantiated and called as such:
     * LFUCache obj = new LFUCache(capacity);
     * int param_1 = obj.Get(key);
     * obj.Put(key,value);
     */
}
