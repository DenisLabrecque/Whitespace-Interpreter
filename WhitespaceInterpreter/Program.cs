using System;

namespace WhitespaceInterpreter
{
   class Program
   {
      static void Main(string[] args)
      {
         string mInput = "   \t\n   \t     \t\n\t   \t\n  \n\n\n";
         Whitespace program = new Whitespace(mInput);
         Console.WriteLine(program);
         try
         {
            Console.WriteLine(program.Run(true));
         }
         catch(Exception e)
         {
            Console.WriteLine(e.Message);
         }
      }
   }
}
