using System;

namespace WhitespaceInterpreter
{
   class Program
   {
      static void Main(string[] args)
      {
         string mInput = "   \t  \t\t\t\t\n[push-O]   \t  \t\t  \n[push-L] \n [duplicate]   \t   \t \t\n[push-E]   \t  \t   \n[push-H]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter] \n\n[discard]\t\n  [output-letter]\n\n\n[halt]";
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
