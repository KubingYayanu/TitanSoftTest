using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ApplicantTestin
{
    /// The DataObject class stored with a key
    internal class DataObject
    {
        public DataObject(string str)
        {
            Alphabet = str;
        }

        public string Alphabet { get; set; }

        public int Value { get; set; }

        public DataObject Reference { get; set; }
    }

    internal class Program
    {
        private static Hashtable Data = new Hashtable();

        private static string[] StaticData = new string[] { "X-Ray","Echo","Alpha", "Yankee","Bravo", "Charlie",
            "Delta",    "Hotel", "India", "Juliet", "Foxtrot","Sierra",
            "Mike","Kilo", "Lima",  "November", "Oscar", "Papa", "Qubec",
            "Romeo",  "Tango","Golf", "Uniform", "Victor", "Whisky",
             "Zulu"};

        private static void Main(string[] args)
        {
            for (int i = 0; i < StaticData.Length; i++)
                Data.Add(StaticData[i].ToLower(), new DataObject(StaticData[i]));
            while (true)
            {
                PrintSortedData();
                Console.WriteLine();
                Console.Write("> ");
                string str = Console.ReadLine();
                string[] strs = str.Split(' ');

                if (strs[0] == "q")
                    break;
                else if (strs[0] == "printv")
                    PrintSortedDataByValue();
                else if (strs[0] == "print")
                    PrintSortedData();
                else if (strs[0] == "inc")
                    Increase(strs[1]);
                else if (strs[0] == "dec")
                    Decrease(strs[1]);
                else if (strs[0] == "swap")
                    Swap(strs[1], strs[2]);
                else if (strs[0] == "ref")
                    Ref(strs[1], strs[2]);
                else if (strs[0] == "unref")
                    UnRef(strs[1]);
            }
        }

        /// <summary>
        /// Create a reference from one data object to another.
        /// </summary>
        /// <param name="key1">The object to create the reference on</param>
        /// <param name="key2">The reference object</param>
        private static void Ref(string key1, string key2)
        {
            var obj1 = Data[key1] as DataObject;
            var obj2 = Data[key2] as DataObject;

            if (obj1 != null && obj2 != null)
            {
                obj1.Reference = obj2;
            }
        }

        /// <summary>
        /// Removes an object reference on the object specified.
        /// </summary>
        /// <param name="key">The object to remove the reference from</param>
        private static void UnRef(string key)
        {
            var obj = Data[key] as DataObject;
            if (obj != null)
            {
                obj.Reference = null;
            }
        }

        /// <summary>
        /// Swap the data objects stored in the keys specified
        /// </summary>
        private static void Swap(string key1, string key2)
        {
            var obj1 = Data[key1] as DataObject;
            var obj2 = Data[key2] as DataObject;
            if (obj1 != null && obj2 != null)
            {
                Data[key1] = obj2;
                Data[key2] = obj1;
            }
        }

        /// <summary>
        /// Decrease the Value field by 1 of the
        /// data object stored with the key specified
        /// </summary>
        private static void Decrease(string key)
        {
            var obj = Data[key] as DataObject;
            if (obj != null)
            {
                obj.Value--;
            }
        }

        /// <summary>
        /// Increase the Value field by 1 of the
        /// data object stored with the key specified
        /// </summary>
        private static void Increase(string key)
        {
            var obj = Data[key] as DataObject;
            if (obj != null)
            {
                obj.Value++;
            }
        }

        /// <summary>
        /// Prints the information in the Data hashtable to the console.
        /// Output should be sorted by key
        /// References should be printed between '<' and '>'
        /// The output should look like the following :
        ///
        ///
        /// Alpha...... -3
        /// Bravo...... 2
        /// Charlie.... <Zulu>
        /// Delta...... 1
        /// Echo....... <Alpha>
        /// --etc---
        ///
        /// </summary>
        private static void PrintSortedData()
        {
            var orderedKeys = Data.Keys.Cast<string>().OrderBy(x => x);

            Console.WriteLine("> PrintSortedData...");
            foreach (var key in orderedKeys)
            {
                var obj = Data[key] as DataObject;
                PrintResult(obj);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the information in the Data hashtable to the console.
        /// Output should be sorted by stored value
        /// References should be printed between '<' and '>'
        /// Sorting order start from max to min, larger value takes priority.
        /// The output should look like the following :
        ///
        ///
        /// Bravo...... 100
        /// Echo...... 99
        /// Zulu...... 98
        /// Charlie.... <Zulu>
        /// Delta...... 34
        /// Echo....... 33
        /// Alpha...... <Echo>
        /// --etc---
        ///
        /// </summary>
        private static void PrintSortedDataByValue()
        {
            var array = new DataObject[Data.Count];
            Data.Values.CopyTo(array, 0);
            var list = array.ToList();
            list.Sort(new DataObjectComparer());

            Console.WriteLine("> PrintSortedDataByValue...");
            foreach (var value in list)
            {
                PrintResult(value);
            }
            Console.WriteLine();
        }

        private static void PrintResult(DataObject obj)
        {
            var prefix = obj.Alphabet.PadRight(11, '.');
            var result = obj.Reference == null
                ? obj.Value.ToString()
                : $"<{obj.Reference.Alphabet}>";
            Console.WriteLine($"{prefix} {result}");
        }
    }

    internal class DataObjectComparer : IComparer<DataObject>
    {
        public int Compare(DataObject x, DataObject y) => y.Value.CompareTo(x.Value);
    }
}