using RepoDb.Science.UnitTests;
using RepoDb.Science.UnitTests.Helpers;

InspectionTests.Run();
InspectionNegativeTests.Run();
InspectionHelperTests.Run();
OperationHelperTests.Run();
SelectionTests.Run();
CleaningTests.Run();
ForwardFillTests.Run();
BackwardFillTests.Run();
TransformationTests.Run();
AggregationTests.Run();
CombiningTests.Run();
MachineLearningTests.Run();
ValidationTests.Run();
WorkflowTests.Run();
Console.WriteLine("All DataFrame tests passed.");
