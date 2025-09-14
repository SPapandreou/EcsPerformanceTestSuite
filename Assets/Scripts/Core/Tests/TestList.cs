using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Tests
{
    public class TestList
    {
        public List<string> Tests { get; set; } = new();

        public static TestList Create(bool includeAll = true)
        {
            var allTestTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t =>
                typeof(TestCase).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            var testList = new TestList();

            if (includeAll)
            {
                testList.Tests.Add("all");    
            }
            
            testList.Tests.AddRange(allTestTypes.Select(t => t.Name));
            
            return testList;
        }
    }
}