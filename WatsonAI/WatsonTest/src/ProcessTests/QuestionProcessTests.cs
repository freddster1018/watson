using Xunit;
using WatsonAI;
using System.Collections.Generic;
using System.Linq;

namespace WatsonTest
{
  /// <summary>
  /// Tests for the processing of questions.
  /// </summary>
  /// <remarks>
  /// Dobj or Subj in test name means a question which is looking for the respective part.
  /// </remarks>
  public class QuestionProcessTests
  {
    static private readonly KnowledgeBuilder builder;
    static private readonly QuestionProcess questionProcess;
    static private readonly Associations associations;

    static QuestionProcessTests()
    {
      var entityBuilder = new EntityBuilder()
      {
        "actress", "earl", "murderer", "dave", "herbology", "study", "belonging"
      };
      var verbBuilder = new VerbBuilder()
      {
        "kill", "love", "be", "study", "contain", "own"
      };
      builder = new KnowledgeBuilder(entityBuilder, verbBuilder)
      {
        { "actress" , "kill", "earl" },
        { "actress" , "study", "herbology" },
        { "actress" , "be", Object.Direct("murderer"), Object.Indirect("of", "earl") },
        { "earl" , "love", "dave" },
        { "study" , "contain", "actress" },
        { "earl" , "own", "belonging" }
      };
      associations = builder.Associations;
      questionProcess = new QuestionProcess(new Parser(), builder.Knowledge, new Thesaurus(associations), builder.Associations);
    }

    [Fact]
    public void SubjectActiveCase()
    {
      //(TOP (SBARQ (WHNP (WP Who)) (SQ (VP (VBZ is) (NP (DT the) (NN murderer)))) (. ?)))
      var input = "Who is the murderer?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "actress" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void IObjActiveCase()
    {
      //(TOP (SBARQ (WHNP (WP Who)) (SQ (VP (VBZ is) (NP (DT the) (NN murderer)))) (. ?)))
      var input = "Who is the murderer of the earl?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "actress" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    //[Fact]
    //public void UnnamedTestCase0()
    //{
    //  var input = "The murderer is whom?";
    //  var answers = questionProcess.GetEntityAnswers(input);
    //  var expectedAnswers = new[] { "actress" };
    //  AssertEntityEquals(expectedAnswers, answers);
    //}

    [Fact]
    public void ActiveSubjWhoCase()
    {
      //(TOP (SBARQ (WHNP (WP Who)) (SQ (VP (VBD killed) (NP (DT the) (NN earl)))) (. ?)))
      var input = "Who killed the earl?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "actress" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "Who killed the actress?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new string[0];
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void ActiveDobjWhoCase()
    {
      //(TOP (SBARQ (WHNP (WP Who)) (SQ (VBD did) (NP (DT the) (NN actress)) (VP (VB kill))) (. ?)))
      var input = "Who did the actress kill?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "earl" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "Who did the earl kill?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new string[0];
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void PassiveDobjWhoCase()
    {
      //(TOP (SBARQ (WHNP (WP Who)) (SQ (VP (VBD was) (VP (VBN killed) (PP (IN by) (NP (DT the) (NN actress)))))) (. ?)))
      var input = "Who was killed by the actress?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "earl" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "Who was killed by the earl?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new string[0];
      AssertEntityEquals(expectedAnswers, answers);
    }

    //[Fact]
    //public void UnnamedTestCase6()
    //{
    //  var input = "The murderer of the earl is whom?";
    //  var answers = questionProcess.GetEntityAnswers(input);
    //  var expectedAnswers = new[] { "actress" };
    //  AssertEntityEquals(expectedAnswers, answers);

    //  input = "The murderer of the actress is whom?";
    //  answers = questionProcess.GetEntityAnswers(input);
    //  expectedAnswers = new string[0];
    //  AssertEntityEquals(expectedAnswers, answers);
    //}

    //[Fact]
    //public void UnnamedTestCase7()
    //{
    //  var input = "Who is the killer of the earl?";
    //  var answers = questionProcess.GetEntityAnswers(input);
    //  var expectedAnswers = new[] { "actress" };
    //  AssertEntityEquals(expectedAnswers, answers);

    //  input = "Who is the killer of the actress?";
    //  answers = questionProcess.GetEntityAnswers(input);
    //  expectedAnswers = new string[0];
    //  AssertEntityEquals(expectedAnswers, answers);
    //}

    [Fact]
    public void ActiveBoolean()
    {
      //(TOP (SQ (VBZ Is) (NP (DT the) (NN actress)) (NP (DT the) (NN murderer)) (. ?)))
      var input = "Is the actress the murderer?";
      var answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      input = "Is the earl the murderer?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.False(answer);
    }

    [Fact]
    public void ActiveBooleanIObj()
    {
      var input = "Is the actress the murderer of the earl?";
      var answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      input = "Is the earl the murderer of the actress?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.False(answer);
    }

    [Fact]
    public void PassiveBoolean()
    {
      //(TOP (SQ (VBD Did) (NP (DT the) (NNS actres)) (VP (VB kill) (NP (DT the) (NN earl))) (. ?)))
      var input = "Did the actress kill the earl?";
      var answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      //(TOP (SQ (VBZ Does) (NP (DT the) (NN earl)) (VP (VB own) (NP (DT the) (NNS belongings))) (. ?)))
      input = "Does the earl own the belongings?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      input = "Did the earl kill the actress?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.False(answer);
    }

    //[Fact]
    //public void UnnamedTestCase10()
    //{
    //  var input = "Did the earl die at the hands of the actress?";
    //  var answer = questionProcess.GetBooleanAnswer(input);
    //  Assert.True(answer);

    //  input = "Did the actress die at the hands of the earl?";
    //  answer = questionProcess.GetBooleanAnswer(input);
    //  Assert.False(answer);
    //}

    [Fact]
    public void UnnamedTestCase11()
    {
      var input = "Was the earl killed by the actress?";
      var answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      input = "Was the actress killed by the earl?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.False(answer);
    }

    [Fact]
    public void UnnamedTestCase12()
    {
      var input = "The actress killed the earl?";
      var answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      input = "The earl killed the actress?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.False(answer);
    }

    [Fact]
    public void WasBoolean()
    {
      //(TOP (SQ (VBD Was) (NP (DT the) (NN earl)) (VP (VBN killed)) (. ?)))
      var input = "Was the earl killed?";
      var answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      input = "Was the actress killed?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.False(answer);
    }

    [Fact]
    public void UnnamedTestCase13()
    {
      var input = "Is the actress the person that killed the earl?";
      var answer = questionProcess.GetBooleanAnswer(input);
      Assert.True(answer);

      input = "Is earl the person that killed the actress?";
      answer = questionProcess.GetBooleanAnswer(input);
      Assert.False(answer);
    }

    [Fact]
    public void UnnamedTestCase14()
    {
      var input = "Could you tell me who killed the earl?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "actress" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "Could you tell me who killed the actress?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new string[0];
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void UnnamedTestCase15()
    {
      var input = "Who is the murderer of the earl?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "actress" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "Who is the murderer of the earl?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new string[0];
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void UnnamedTestCase16()
    {
      var input = "Who is the earl's murderer?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "actress" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "Who is the earl's murderer?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new string[0];
      AssertEntityEquals(expectedAnswers, answers);
    }

    //[Fact]
    //public void AmbitiousTestCase1()
    //{
    //  var input = "Where is the earl's murderer?";
    //  var answers = questionProcess.GetEntityAnswers(input);
    //  var expectedAnswers = new[] { "study" };
    //  AssertEntityEquals(expectedAnswers, answers);

    //  input = "Where is the actress's murderer?";
    //  answers = questionProcess.GetEntityAnswers(input);
    //  expectedAnswers = new string[0];
    //  AssertEntityEquals(expectedAnswers, answers);
    //}

    // Lover of means that he loves
    //[Fact]
    //public void AmbitiousTestCase2()
    //{
    //  var input = "Who is the lover of dave?";
    //  var answers = questionProcess.GetEntityAnswers(input);
    //  var expectedAnswers = new[] { "earl" };
    //  AssertEntityEquals(expectedAnswers, answers);

    //  input = "Who is the lover of dave?";
    //  answers = questionProcess.GetEntityAnswers(input);
    //  expectedAnswers = new string[0];
    //  AssertEntityEquals(expectedAnswers, answers);
    //}

    [Fact]
    public void Where1()
    {
      //(TOP (SBARQ (WHADVP (WRB Where)) (SQ (VP (VBZ is)) (NP (DT the) (NN actress))) (. ?)))
      var input = "Where is the actress?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "study" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void Where2()
    {
      //(TOP (SBARQ (WHADVP (WRB Where)) (SQ (MD can) (NP (PRP I)) (VP (VB find) (NP (DT the) (NN actress)))) (. ?)))
      var input = "Where can I find the actress?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "study" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void Where3()
    {
      var input = "What is the location of the actress?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "study" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void Where4()
    {
      var input = "Where can I look for the actress?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "study" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void Where5()
    {
      var input = "Where's the actress?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "study" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void Where6()
    {
      var input = "What room is the actress in?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "study" };
      AssertEntityEquals(expectedAnswers, answers);
    }

    [Fact]
    public void SameWordForNounAndVerb()
    {
      var input = "What does the actress study?";
      var answers = questionProcess.GetEntityAnswers(input);
      var expectedAnswers = new[] { "herbology" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "What does the actress learn?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new[] { "herbology" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "Who studies herbology?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new[] { "actress" };
      AssertEntityEquals(expectedAnswers, answers);

      input = "What does the earl study?";
      answers = questionProcess.GetEntityAnswers(input);
      expectedAnswers = new string[0];
      AssertEntityEquals(expectedAnswers, answers);
    }

    // Asserts that two collections of entities is unorderedly equal to a list of entities,
    // specified by their names
    private void AssertEntityEquals(string[] expectedAnswers, IEnumerable<Entity> answers)
    {
      Assert.Equal(GetEntitiesFromNames(expectedAnswers).OrderBy(e => (uint)e),
        answers.OrderBy(e => (uint)e));
    }

    private Entity[] GetEntitiesFromNames(string[] entityNames)
    {
      var entities = new List<Entity>();
      foreach (var name in entityNames)
      {
        var entity = associations.UncheckedGetEntity(name);
        entities.Add(entity);
      }
      return entities.ToArray();
    }

    private Verb[] GetVerbsFromNames(string[] verbNames)
    {
      var verbs = new List<Verb>();
      foreach (var name in verbNames)
      {
        var verb = associations.UncheckedGetVerb(name);
        verbs.Add(verb);
      }
      return verbs.ToArray();
    }
  }
}
