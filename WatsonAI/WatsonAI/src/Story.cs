﻿using System;
using System.Collections.Generic;

namespace WatsonAI
{
  /// <summary>
  /// Creates a Knowledge object and its Associations based on hard-coded story
  /// </summary>
  public static class Story
  {
    private static readonly KnowledgeBuilder universeKnowledgeBuilder;
    public static List<Character> Characters { get; }
    public static Knowledge Knowledge { get; }
    public static Associations Associations { get; }

    static Story()
    {
      var entities = new EntityBuilder {
        "actress", "butler", "countess", "earl",
        "gangster", "money", "promotion", "money",
        "study", "will", "belonging", "letter",
        "colonel", "book", "arsenic", "barbital",
        "nightshade", "murderer", "dead", "music", "chocolate",
        "house", "earth", "england", "universe", "dorset", "policeman"
      };
      var verbs = new VerbBuilder {
        "poison", "marry", "owe", "employ",
        "inherit", "prevent", "launder", "contain",
        "own", "resent", "do", "blackmail",
        "use", "kill", "be", "love", "like"};
      universeKnowledgeBuilder = new KnowledgeBuilder(entities, verbs)
      {
        //{"actress", "be", "murderer"},
        {"earl", "love", "countess"},
        {"countess", "love", "earl"},
        {"countess", "love", "chocolate"},
        {"earl", "like", "music"},
        {"actress", "poison", "earl"},
        {"earl", "be", "dead"},
        {"actress", "kill", "earl"},
        {"countess", "marry", "earl"},
        {"earl", "marry", "countess"},
        {"earl", "owe", "gangster"},
        {"earl", "employ", "butler"},
        {"actress", "inherit", "money"},
        {"earl", "prevent", "promotion"},
        {"butler", "launder", "money"},
        {"study", "contain", "will"},
        {"house", "contain", "study"},
        {"dorset", "contain", "house"},
        {"england", "contain", "house"},
        {"earth", "contain", "england"},
        {"universe", "contain", "england"},
        {"earl", "own", "belonging"},
        {"belonging", "contain", "letter"},
        {"colonel", "resent", "earl"},
        {"butler", "do", "book"}, // butler do book?
        {"gangster", "blackmail", "butler"},
        {"butler", "own", "arsenic"},
        {"earl", "use", "barbital"},
        {"belonging", "contain", "nightshade"},
      };

      Knowledge = universeKnowledgeBuilder.Knowledge;
      Associations = universeKnowledgeBuilder.Associations;

      Characters = new List<Character>
      {
        new Character("actress", true),
        new Character("countess", false),
        new Character("colonel", false),
        new Character("gangster", false),
        new Character("policeman", false),
        new Character("butler", false)
      };

      var characterKnowledgeBuilders = new List<KnowledgeBuilder>
      {
        new KnowledgeBuilder(entities, verbs)
        {
          {"actress", "poison", "earl"},
          {"earl", "love", "countess"},
          {"countess", "love", "earl"},
          {"earl", "like", "music"},
          {"earl", "be", "dead"},
          {"actress", "kill", "earl"},
          {"countess", "marry", "earl"},
          {"earl", "marry", "countess"},
          {"earl", "employ", "butler"},
          {"actress", "inherit", "money"},
          {"study", "contain", "will"},
          {"earl", "own", "belonging"},
          {"earl", "use", "barbital"}
        },
        new KnowledgeBuilder(entities, verbs)
        {
          {"earl", "love", "countess"},
          {"countess", "love", "earl"},
          {"earl", "like", "music"},
          {"earl", "be", "dead"},
          {"actress", "kill", "earl"},
          {"countess", "marry", "earl"},
          {"earl", "marry", "countess"},
          {"earl", "employ", "butler"},
          {"actress", "inherit", "money"},
          {"study", "contain", "will"},
          {"earl", "own", "belonging"},
          {"earl", "use", "barbital"},
          {"countess", "love", "chocolate"},
          {"belonging", "contain", "letter"},
          {"belonging", "contain", "nightshade"}
        },
        new KnowledgeBuilder(entities, verbs)
        {
          {"actress", "poison", "earl"},
          {"earl", "love", "countess"},
          {"countess", "love", "earl"},
          {"earl", "like", "music"},
          {"earl", "be", "dead"},
          {"actress", "kill", "earl"},
          {"countess", "marry", "earl"},
          {"earl", "marry", "countess"},
          {"earl", "employ", "butler"},
          {"actress", "inherit", "money"},
          {"study", "contain", "will"},
          {"earl", "own", "belonging"},
          {"earl", "use", "barbital"}
        },
        new KnowledgeBuilder(entities, verbs)
        {
          {"actress", "poison", "earl"},
          {"earl", "love", "countess"},
          {"countess", "love", "earl"},
          {"earl", "like", "music"},
          {"earl", "be", "dead"},
          {"actress", "kill", "earl"},
          {"countess", "marry", "earl"},
          {"earl", "marry", "countess"},
          {"earl", "employ", "butler"},
          {"actress", "inherit", "money"},
          {"study", "contain", "will"},
          {"earl", "own", "belonging"},
          {"earl", "use", "barbital"}
        },
        new KnowledgeBuilder(entities, verbs)
        {
          {"actress", "poison", "earl"},
          {"earl", "love", "countess"},
          {"countess", "love", "earl"},
          {"earl", "like", "music"},
          {"earl", "be", "dead"},
          {"actress", "kill", "earl"},
          {"countess", "marry", "earl"},
          {"earl", "marry", "countess"},
          {"earl", "employ", "butler"},
          {"actress", "inherit", "money"},
          {"study", "contain", "will"},
          {"earl", "own", "belonging"},
          {"earl", "use", "barbital"}
        },
        new KnowledgeBuilder(entities, verbs)
        {
          {"actress", "poison", "earl"},
          {"earl", "love", "countess"},
          {"countess", "love", "earl"},
          {"earl", "like", "music"},
          {"earl", "be", "dead"},
          {"actress", "kill", "earl"},
          {"countess", "marry", "earl"},
          {"earl", "marry", "countess"},
          {"earl", "employ", "butler"},
          {"actress", "inherit", "money"},
          {"study", "contain", "will"},
          {"earl", "own", "belonging"},
          {"earl", "use", "barbital"}
        }
      };

      for (int i = 0; i < Characters.Count; i++)
      {
        Characters[i].Knowledge = characterKnowledgeBuilders[i].Knowledge;
      }
    }
  }
}