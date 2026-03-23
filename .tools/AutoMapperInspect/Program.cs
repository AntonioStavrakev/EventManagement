using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        try
        {
            var asm = Assembly.Load("AutoMapper");
            var t = asm.GetType("AutoMapper.MapperConfiguration");
            if (t == null) { Console.WriteLine("Type not found"); return; }
            Console.WriteLine("Constructors for AutoMapper.MapperConfiguration:");
            foreach (var c in t.GetConstructors())
            {
                Console.WriteLine(c.ToString());
                foreach (var p in c.GetParameters())
                    Console.WriteLine("  " + p.ParameterType.FullName + " " + p.Name);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex);
        }
    }
}
