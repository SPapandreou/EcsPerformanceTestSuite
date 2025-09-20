using System;
using System.Collections.Generic;
using System.Linq;
using Core.Main;
using UnityEngine.UIElements;

namespace Core.Tests
{
    public class TestCaseFactory
    {
        private readonly VisualTreeAsset _tableRowTemplate;
        
        public TestCaseFactory(VisualTreeAsset tableRowTemplate)
        {
            _tableRowTemplate = tableRowTemplate;
        }

        public TestCase CreateTestCase(Type type)
        {
            var testCase = Activator.CreateInstance(type, new object[] {_tableRowTemplate}) as TestCase;
            return testCase;
        }

        public TestCase CreateTestCase(TestRunFileEntry entry)
        {
            var type = GetType(entry.TestCase);
            
            if (type == null)
            {
                throw new InvalidOperationException($"Invalid test case file. Could not find type {entry.TestCase}");
            }
            var testCase = Activator.CreateInstance(type, _tableRowTemplate, entry) as TestCase;
            return testCase;
        }

        private Type GetType(string name)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = asm.GetTypes()
                        .FirstOrDefault(t => t.Name == name);
                    if (type != null)
                        return type;
                }
                catch
                {
                    // ignore assemblies that can't be reflected
                }
            }

            return null;
        }

        public static List<TestListRow> GetTestCases()
        {
            return GetTestCaseTypes().Select(type => new TestListRow(type)).ToList();
        }

        public static List<Type> GetTestCaseTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t =>
                typeof(TestCase).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface).ToList();
        }
    }
}