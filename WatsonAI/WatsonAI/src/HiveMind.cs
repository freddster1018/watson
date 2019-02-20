﻿using Syn.WordNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WatsonAI 
{
  public class HiveMind
  {
    private KnowledgeGraph mainGraph;
    private List<KnowledgeGraph> subGraphs;

    public HiveMind(KnowledgeGraph mainGraph, List<KnowledgeGraph> subGraphs)
    {
      this.mainGraph = mainGraph;
      this.subGraphs = subGraphs;
    }

    /*public static Universe CheckedNew(KnowledgeGraph mainGraph, List<KnowledgeGraph> subGraphs)
      => subGraphs.All(g => IsSubGraph(g, mainGraph)) ? new Universe(mainGraph, subGraphs) : null;*/

    public static HiveMind CheckedNew(KnowledgeGraph mainGraph, List<KnowledgeGraph> subGraphs)
    {
      foreach (var subGraph in subGraphs)
      {
        if (!IsSubGraph(subGraph, mainGraph))
        {
          return null;
        }
      }
      return new HiveMind(mainGraph, subGraphs);
    }

    // "This works" - Jago 2019
    private static bool IsSubGraph(KnowledgeGraph subGraph, KnowledgeGraph mainGraph)
    {
      return Enumerable.Zip(
        mainGraph.AllRelations(),
        subGraph.AllRelations(),
        (main, sub) => main.relation.Contains(sub.relation)).All(x => x);
    }
  }

  [Serializable]
  public class Associations
  {
    public string[] entityNames { get; }

    public string[] relationNames { get; }

    public WordNetEngine wordNet { get; }

    public Associations(int entityCount, int relationCount)
    {
      entityNames = new string[entityCount];
      relationNames = new string[relationCount];
      wordNet = new WordNetEngine();

      string directory = $"{Directory.GetCurrentDirectory()}/wordnet/dict/";
      Console.WriteLine("Loading database...");
      Console.WriteLine(directory);
            Console.WriteLine("cat");
      wordNet.LoadFromDirectory(directory);
      Console.WriteLine("Load completed.");
    }

    public string NameOf(Entity entity)
      => entityNames[(int) entity];

    public void SetNameOf(Entity entity, string name)
    {
      entityNames[(int)entity] = name;
    }

    public string NameOf(Relation relation)
      => relationNames[(int) relation];

    public void SetNameOf(SingleRelation relation, string name)
    {
      relationNames[(int)relation] = name;
    }

    public bool Describes(string word, Entity entity)
    {
      //if (entityNames[(int)entity].ToLower().Equals(word.ToLower()))
      //{
      //  return true;
      //}

      
      var synSetList = wordNet.GetSynSets(entityNames[(int)entity].ToLower());
      if (synSetList.Count == 0)
      {
        return false;
      }

      foreach (var synSet in synSetList)
      {
        var words = string.Join(", ", synSet.Words);
        Console.WriteLine($"\nWords: {words}");
        if (synSet.Words.Contains(word.ToLower()))
        {
          return true;
        }


        
        //Console.WriteLine($"POS: {synSet.PartOfSpeech}");
        //Console.WriteLine($"Gloss: {synSet.Gloss}");
      }
      return false;

    }

    public bool Describes(string word, Relation relation)
    {
      foreach (var singleRelation in relation.SingleRelations())
      {
        if (relationNames[(int)singleRelation].ToLower().Equals(word.ToLower()))
        {
          return true;
        }
      }
      return false;
    }
  }

  /// <summary>
  /// Utility class for saving a knowledge graph and associated configurations in one file.
  /// </summary>
  [Serializable]
  public class Universe
  {
    public KnowledgeGraph knowledgeGraph { get; }

    public Associations associations { get; }

    public Universe(KnowledgeGraph knowledgeGraph, Associations associations)
    {
      this.knowledgeGraph = knowledgeGraph;
      this.associations = associations;
    }
  }
}
