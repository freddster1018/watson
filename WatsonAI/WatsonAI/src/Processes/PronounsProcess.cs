﻿using OpenNLP.Tools.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

using static WatsonAI.Patterns;

namespace WatsonAI
{
  /// <summary>
  /// Implements a pre-process and post-process that remove and add, respectively, pronouns.
  /// </summary>
  public class PronounsProcess : IPreProcess, IPostProcess
  {
    private readonly Character character;
    private readonly Memory memory;
    private readonly Parser parser;
    private readonly List<Character> characters;


    public PronounsProcess(Character character, List<Character> characters, Parser parser)
    {
      this.character = character;
      this.characters = characters;
      this.parser = parser;
    }

    public PronounsProcess(Character character, List<Character> characters, Memory memory, Parser parser)
    {
      this.character = character;
      this.characters = characters;
      this.memory = memory;
      this.parser = parser;
    }

    /// <summary>
    /// Implements a pre-process that replaces pronouns with the character name in the input stream.
    /// </summary>
    /// <param name="tokens">A reference to a list of tokens to act on.</param>
    public void PreProcess(ref List<string> tokens)
    {
      string entity = "";
      var replacingItWord = CheckForItWord(tokens, out entity);
      var inputCharacters = FindCharactersInInput(tokens);

      for (int i = 0; i < tokens.Count; i++)
      {
        if (i < tokens.Count - 1)
        {
          ReplaceWords(new List<string> { "do", "you" }, new List<string> { "does", this.character.Name }, tokens, i);
          ReplaceWords(new List<string> { "you", "are" }, new List<string> { this.character.Name, "is" }, tokens, i);
          ReplaceWords(new List<string> { "are", "you" }, new List<string> { "is", this.character.Name }, tokens, i);

          ReplaceWords(new List<string> { "do", "I" }, new List<string> { "does", "Watson" }, tokens, i);
          ReplaceWords(new List<string> { "I", "am" }, new List<string> { "Watson", "is" }, tokens, i);
          ReplaceWords(new List<string> { "I", "'m" }, new List<string> { "Watson", "is" }, tokens, i);
          ReplaceWords(new List<string> { "am", "I" }, new List<string> { "is", "Watson" }, tokens, i);
        }

        ReplaceWords(new List<string> { "your" }, new List<string> { this.character.Name, "'s" }, tokens, i);
        ReplaceWords(new List<string> { "you" }, new List<string> { this.character.Name }, tokens, i);
        ReplaceWords(new List<string> { "I" }, new List<string> { "Watson" }, tokens, i);
        ReplaceWords(new List<string> { "me" }, new List<string> { "Watson" }, tokens, i);
        ReplaceWords(new List<string> { "my" }, new List<string> { "Watson", "'s" }, tokens, i);
        ReplaceWords(new List<string> { "mine" }, new List<string> { "Watson", "'s" }, tokens, i);

        if (replacingItWord)
        {
          ReplaceWords(new List<string> { "it" }, new List<string> { "the", entity }, tokens, i);
        }
        if (inputCharacters.Any())
        {
          var inputCharacter = inputCharacters.First();
          if (inputCharacter.Gender == Gender.Male)
          {
            ReplaceWords(new List<string> { "him" }, new List<string> { inputCharacter.Name }, tokens, i);
            ReplaceWords(new List<string> { "his" }, new List<string> { inputCharacter.Name, "'s" }, tokens, i);
            ReplaceWords(new List<string> { "he" }, new List<string> { inputCharacter.Name }, tokens, i);
          }
          if (inputCharacter.Gender == Gender.Female)
          {
            ReplaceWords(new List<string> { "her" }, new List<string> { inputCharacter.Name }, tokens, i);
            ReplaceWords(new List<string> { "her", "'s" }, new List<string> { inputCharacter.Name, "'s" }, tokens, i);
            ReplaceWords(new List<string> { "she" }, new List<string> { inputCharacter.Name }, tokens, i);
          }
          if (inputCharacter.Gender == Gender.Other || inputCharacter.Gender == Gender.Male || inputCharacter.Gender == Gender.Female)
          {
            ReplaceWords(new List<string> { "them" }, new List<string> { inputCharacter.Name }, tokens, i);
            ReplaceWords(new List<string> { "their", "'s" }, new List<string> { inputCharacter.Name, "'s" }, tokens, i);
            if (inputCharacters.Count == 1)
            {
              ReplaceWords(new List<string> { "they" }, new List<string> { inputCharacter.Name }, tokens, i);
              ReplaceWords(new List<string> { "are", "they" }, new List<string> { "is", inputCharacter.Name }, tokens, i);
            }
          }
        }
      }
      

      //TODO: Else here is a good place to introduce the failstate.
    }

    /// <summary>
    /// Implements a post-process that replaces character names with pronouns in the ouput stream.
    /// </summary>
    /// <param name="tokens">A reference to a list of tokens to act on.</param>
    public void PostProcess(ref List<string> tokens)
    {
      for (int i = 0; i < tokens.Count; i++)
      {
        ReplaceWords(new List<string> { "Watson" }, new List<string> { "you" }, tokens, i);
        ReplaceWords(new List<string> { "the", character.Name }, new List<string> { "me" }, tokens, i);
        ReplaceWords(new List<string> { character.Name }, new List<string> { "me" }, tokens, i);
      }
    }

    private void ReplaceWords(List<string> originals, List<string> replacements, List<string> tokens, int i)
    {
      if (i + originals.Count <= tokens.Count)
      {
        var originalTokens = tokens.GetRange(i, originals.Count);
        if (originals.Zip(originalTokens, (x, y) => x.Equals(y, StringComparison.OrdinalIgnoreCase)).All(x => x))
        {
          tokens.RemoveRange(i, originals.Count);
          tokens.InsertRange(i, replacements);
        }
      }
    }

    private List<Character> FindCharactersInInput(List<string> tokens)
    {
      var storyCharacters = this.characters;
      var inputCharacters = new List<Character>();
      foreach (var token in tokens)
      {
        foreach (var character in storyCharacters)
        {
          if (token.Equals(character.Name, StringComparison.OrdinalIgnoreCase))
          {
            inputCharacters.Add(character);
          }
        }
      }
      return inputCharacters;
    }

    private bool CheckForItWord(List<string> tokens, out string word)
    {
      if (tokens.Contains("it"))
      {
        var sentenceUpToIt = tokens.Take(tokens.FindIndex(x => x == "it"));
        if (FindItWord(sentenceUpToIt, out word))
        {
          return true;
        }
        else if (this.memory != null)
        {
          var response = memory.GetLastResponse();
          var responseTokens = parser.Tokenize(response);
          if (FindItWord(responseTokens, out word))
          {
            return true;
          }
          var inputTokens = parser.Tokenize(memory.GetLastInput());
          if (FindItWord(inputTokens, out word))
          {
            return true;
          }
        }
      }
      word = "";
      return false;
    }

    private bool FindItWord(IEnumerable<string> tokens, out string word)
    {
      Parse parse;
      var parseExists = parser.Parse(tokens, out parse);

      if (parseExists)
      {
        var top = Branch("TOP");

        var subject = top >= Branch("NN");

        var entity = subject.Match(parse);

        if (entity.HasValue)
        {
          word = entity.Value.First().Value;
          return true;
        }
      }
      word = "";
      return false;
    }
  }
}
