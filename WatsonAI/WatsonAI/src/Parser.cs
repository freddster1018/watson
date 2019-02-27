﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;

namespace WatsonAI
{
  /// <summary>
  /// Parses sentences into grammatical trees.
  /// </summary>
  public class Parser
  {

    private EnglishTreebankParser parser;

    private EnglishRuleBasedTokenizer tokenizer;

    /// <summary>
    /// Initialises a new instance of the <see cref="GameAI.Parser"/> class.
    /// </summary>
    public Parser()
    {
      //var modelPathSen = Directory.GetCurrentDirectory() + "\\res\\Models\\EnglishSD.nbin";
      //var modelPathTok = Directory.GetCurrentDirectory() + "\\res\\Models\\EnglishTok.nbin";
      //var modelPathPos = Directory.GetCurrentDirectory() + "\\res\\Models\\EnglishPOS.nbin";
      var modelPathParse = Directory.GetCurrentDirectory() + "\\res\\Models\\";

      //var tagDictDir = Directory.GetCurrentDirectory() + "\\res\\Models\\Parser\\tagdict";

      this.tokenizer = new EnglishRuleBasedTokenizer(false);
      this.parser = new EnglishTreebankParser(modelPathParse);
    }

    /// <summary>
    /// Generates a parse tree of the gramaitical structure of the sentence.
    /// </summary>
    /// <remarks>
    /// To avoid buggy results, tokenize before parsing, even though parse accepts a string.
    /// </remarks>
    /// <param name="sentence">The sentence.</param>
    /// <returns>A parse tree of the sentence.</returns>
    public Parse Parse(string sentence)
    {
      var tokens = Tokenize(sentence);
      return this.parser.DoParse(tokens);
    }

    /// <summary>
    /// Splits the sentence into tokens (words).
    /// </summary>
    /// <param name="sentence">The sentence to split.</param>
    /// <returns>A list of tokens.</returns>
    public string[] Tokenize(string sentence)
    {
      return this.tokenizer.Tokenize(sentence);
    }
  }
}
