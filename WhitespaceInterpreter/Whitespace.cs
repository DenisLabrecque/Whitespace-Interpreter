using System;
using System.Collections.Generic;
using System.Text;

namespace WhitespaceInterpreter
{
   enum State { IMP, Command, Parameter }
   enum Command { Push, Duplicate, Swap, Discard,
      Add, Subtract, Multiply,
      Store, Retrieve,
      Halt,
      OutputCharacter, OutputNumber }

   /// <summary>
   /// Represents a Whitespace program; implements the stack-based system using C# generics.
   /// Parses and interprets the meaning of Whitespace commands (basic ones for now).
   /// </summary>
   class Whitespace
   {
      public static readonly HashSet<char> ValidCharacters = new HashSet<char>() { ' ', '\n', '\t' };

      string mCommands;
      State mState = State.IMP;
      Stack<int> mStack;
      bool mDebug = false;

      private static readonly Dictionary<string, Dictionary<string, Command>> mCommandDictionary = new Dictionary<string, Dictionary<string, Command>>()
      {
         // Stack manipulation
         { " ", new Dictionary<string, Command>()
            {
            { " ", Command.Push },
            { "\n ", Command.Duplicate },
            { "\n\t", Command.Swap },
            { "\n\n", Command.Discard },
            }
         },
         // Arithmetic
         { "\t ", new Dictionary<string, Command>()
            {
            { "  ", Command.Add },
            { " \t", Command.Subtract },
            { " \n", Command.Multiply },
            }
         },
         // Heap access
         { "\t\t", new Dictionary<string, Command>()
         {
            { " ", Command.Store },
            { "\t", Command.Retrieve }
         }
         },
         // Flow control
         { "\n", new Dictionary<string, Command>() {
            { "\n\n", Command.Halt }
         }
         },
         // IO
         { "\t\n", new Dictionary<string, Command>() {
            { "  ", Command.OutputCharacter },
            { " \t", Command.OutputNumber }
         }
         }
      };

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="commands">A string of Whitespace commands. These values must be correct in sequence.
      /// Non-Whitespace characters are removed.</param>
      public Whitespace(string commands)
      {
         mCommands = StripNonWhitespace(commands);
         mStack = new Stack<int>();
      }

      /// <summary>
      /// Returns the result of running this Whitespace program.
      /// </summary>
      /// <param name="debug">Show a step-by-step list of actions performed by Whitespace.</param>
      /// <returns>The output from running the program.</returns>
      public string Run(bool debug = false)
      {
         mDebug = debug;

         StringBuilder builder = new StringBuilder();
         mState = State.IMP;
         string impKey = string.Empty;
         Command command;

         for (int i = 0; i < mCommands.Length; i++)
         {
            if (mState == State.IMP)
            {
               impKey = FindImp(mCommands.Substring(i, mCommands.Length - i), ref i);
               mState = State.Command;
            }
            else
            {
               command = FindCommand(impKey, mCommands.Substring(i, mCommands.Length - i), ref i);
               if (CommandHasParameter(command))
               {
                  builder.Append(RunCommand(command, mCommands.Substring(i, mCommands.Length - i), ref i));
               }
               else
               {
                  builder.Append(RunCommand(command));
               }
               mState = State.IMP;
            }
         }

         return builder.ToString();
      }

      private bool CommandHasParameter(Command command)
      {
         switch(command)
         {
            case Command.Push:
               return true;
            default:
               return false;
         }
      }

      public static string StripNonWhitespace(string program)
      {
         StringBuilder builder = new StringBuilder();

         foreach(char letter in program)
         {
            if(ValidCharacters.Contains(letter))
            {
               builder.Append(letter);
            }
         }

         return builder.ToString();
      }

      /// <summary>
      /// Run a parameterless command.
      /// </summary>
      /// <param name="command">Command to be run.</param>
      /// <returns>The command name.</returns>
      private string RunCommand(Command command)
      {
         int last;
         int first;
         string output = null;

         switch(command)
         {
            case Command.Add:
               last = mStack.Pop();
               first = mStack.Pop();
               mStack.Push(first + last);
               break;
            case Command.Subtract:
               last = mStack.Pop();
               first = mStack.Pop();
               mStack.Push(first - last);
               break;
            case Command.Multiply:
               last = mStack.Pop();
               first = mStack.Pop();
               mStack.Push(last * first);
               break;
            case Command.Swap:
               last = mStack.Pop();
               first = mStack.Pop();
               mStack.Push(last);
               mStack.Push(first);
               break;
            case Command.OutputCharacter:
               last = mStack.Pop();
               output = "-> " + Convert.ToChar(last);
               break;
            case Command.OutputNumber:
               last = mStack.Pop();
               output = "-> " + last;
               break;
         }

         if (mDebug)
         {
            return Enum.GetName(typeof(Command), command) + output + '\n';
         }
         else
         {
            return output + '\n';
         }
      }

      /// <summary>
      /// Run a parametered command.
      /// </summary>
      /// <param name="command">Command to be run.</param>
      /// <param name="parameter">The parameter string, plus any characters afterwards.</param>
      /// <param name="index">The index of the parameter string in the original program.</param>
      /// <returns>The result of running the command.</returns>
      private string RunCommand(Command command, string parameter, ref int index)
      {
         // Assuming push command
         int sign = 0;
         bool terminated = false;
         StringBuilder binaryString = new StringBuilder();
         
         //Console.WriteLine("Param: " + ToWords(parameter));
         for(int i = 0; i < parameter.Length; i++)
         {
            if(i == 0)
            {
               if(parameter[i] == '\t')
               {
                  sign = -1;
               }
               else if(parameter[i] == ' ')
               {
                  sign = 1;
               }
               else
               {
                  throw new Exception("Unknown parameter for integer sign " + ToWords(parameter));
               }
            }
            else
            {
               if(parameter[i] == '\n')
               {
                  index += i;
                  terminated = true;
                  break;
               }
               else
               {
                  if(parameter[i] == '\t')
                  {
                     binaryString.Append('1');
                  }
                  else
                  {
                     binaryString.Append('0');
                  }
               }
            }
         }

         if(terminated == false)
         {
            throw new Exception("Non-terminating parameter: " + ToWords(parameter));
         }
         
         int conversion = Convert.ToInt32(binaryString.ToString(), 2) * sign;

         mStack.Push(conversion);

         if (mDebug)
         {
            return Enum.GetName(typeof(Command), command) + " " + conversion + '\n';
         }
         else
         {
            return null;
         }
      }

      private Command FindCommand(string impKey, string command, ref int index)
      {
         foreach(string key in mCommandDictionary[impKey].Keys)
         {
            if(command.StartsWith(key))
            {
               //Console.WriteLine("Command " + ToWords(key) + " found");
               index += key.Length - 1;
               return mCommandDictionary[impKey][key];
            }
         }

         throw new Exception("Command not found for IMP " + ToWords(impKey) + ": " + ToWords(command));
      }

      private string FindImp(string imp, ref int index)
      {
         foreach(string key in mCommandDictionary.Keys)
         {
            if(imp.StartsWith(key))
            {
               //Console.WriteLine("IMP " + ToWords(key) + " found");
               index += key.Length - 1;
               return key;
            }
         }

         throw new Exception("IMP not found for string: " + ToWords(imp));
      }

      public static string ToWords(string command)
      {
         StringBuilder builder = new StringBuilder();

         foreach(char letter in command)
         {
            switch(letter)
            {
               case ' ':
                  builder.Append("[Space]");
                  break;
               case '\n':
                  builder.Append("[LF]");
                  break;
               case '\t':
                  builder.Append("[Tab]");
                  break;
               default:
                  break;
            }
         }

         return builder.ToString();
      }

      /// <summary>
      /// Shows the operations inside the Whitespace program by replacing characters
      /// with [Space], [Tab], or [LF].
      /// </summary>
      /// <returns>A debugging list of operations.</returns>
      public override string ToString()
      {
         return ToWords(mCommands);
      }
   }
}
