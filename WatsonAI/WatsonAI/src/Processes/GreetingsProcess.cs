﻿using OpenNLP.Tools.Parser;
using System;
using System.Collections.Generic;

namespace WatsonAI
{
  /// <summary>
  /// Engine for processing greetings at the start of the input.
  /// </summary>
  public class GreetingsProcess : IProcess
  {
    private Parser parser;
    private Character character;
    private Knowledge kg;
    private Thesaurus thesaurus;
    private Associations associations;


    public GreetingsProcess(Parser parse, Thesaurus thesaurus) 
    {
      this.parser = parse;
      this.thesaurus = thesaurus;

    }

    public GreetingsProcess(Parser parse, Character character, Knowledge kg, Thesaurus thesaurus, Associations associations) 
    {
      this.parser = parse;
      this.character = character;
      this.kg = kg;
      this.thesaurus = thesaurus;
      this.associations = associations;
    }

    /// <summary>
    /// Checks for a greeting at the start of the string. 
    /// and adds a greeting to the output.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <returns>Mutated stream.</returns>
    public Stream Process(Stream stream)
    {
      var streamNew = stream.Clone();
      string word;
      streamNew.NextToken(out word);

      Parse tree;
      if (!parser.Parse(word, out tree))
      {
        return stream;
      }

      var top = new Branch("TOP");
      var hello = new Descendant<string>(top, new Word(thesaurus, "hello"));
      var result = hello.Match(tree);

      if (result.HasValue)
      {
        var listOfGreetings = new List<string>
          {"Hey", 
           "Hi",
           "Wassup",
           "Good evening",
           "G'day",
           "Salutations,"
        };
        Random rnd = new Random();
        Random watson = new Random();

        var index = rnd.Next(listOfGreetings.Count);
        if (watson.Next(2) == 1) 
        { 
          streamNew.AppendOutput(listOfGreetings[index] + ", Watson" );
        }
        else 
        {
          streamNew.AppendOutput(listOfGreetings[index]);
        }
        return streamNew;
      }
      return stream;
    }
  }
}
