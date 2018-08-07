Main TODO's:

**conceptual todo's**:

The main thing left to think about is how to elegantly handle turning costs:

- Can we combine or reuse existing methods of solving?
- Can we perhaps use a regular solver and then 'add' in turning costs after?
- Can we optmize a fixed sequence efficiëntly (related to above).

**make sure to check to TODO's in the code**

- Convert *all* TSP solvers to solvers that can handle sub-problems of matrices.
- Take into account visit costs in all solvers.
  - TSP
  - TSP-TW
  - ...
- Unittest solver registry and solverhooks.
- Unittest model mappers.
- Unittest model validation and implement if needed.
- Make sure the public API makes sense, internalize everything except what we can use in solvers/strategies.
   - We can verify this by implement or starting a BWaste-specific implementation.
- Unittest Shared.CheapestInsertion or verify that is tested.
- Unittest Shared.HillClimbing3Opt or verify that it is tested.
- Remove explicit reference on Reminiscence once Itinero is published.
- Implement nice tostring on the model objects.
- Add the travel weight and visit weights to TSP's.
- Have an other look at sequences and seq and their usage in the exchange operator.
- Create a way to update first of a tour (and use this in swap).
- Test CVRP solver with a 'null' arrival location.
- local2shift for time windows:
  - Implement a best-improvement version of the local1shift operator for timewindows. Currently it's first-improvement.
  - Make sure this is compatible with visit-lists and cases where the problem has more visits than the solution. This is needed for use in an STSP for example.
- Fix performance issues in EAX.
- code review:
   - C# 7.3 features everywhere!
   - customers no, visits yes!
   - only use 'time' on timewindow-specific stuff, otherwise use weight.
   - don't forget tests.
- change resulting stop meta in routes using:
   - visit: the visit id from the original model.
   - index: remove the index, it's confusing.
   - position: the position in the current tour.
 - TimeWindows:
   - Unify unlimited and IsEmpty.
 - Functional tests: conver all problems in CVRP and CVRP-ND.
 - Implement intermediate results handling on all solvers.


 Ideas for speeding up tour():

 - Implement an O(1) contains, this can be done by always proper resetting NEXT.
 - Then it's very fast to update contains on insertafter for example and that also becomes O(1).

 Ideas for speeding up GA:

 - Use multithreading everywhere but make sure not to use multiple operators on the same candidate at the same time.

 Ideas for speeding up CI:

 - Limit the visits being considered to only nearest neighbours.
 - Don't use pair but GetNext() on tour, this way we can exclude visits too far 'from' first before even considering 'to'.
 
 General ideas:
 - See if we can (quickly) figure out if the shortest path between two locations is along other locations.
 - Build a generic highly optimized tour building heuristic based on CI and 3OPT.
 
 
 Best found spijkenisse-5400:
 
 route_0:3321.945s | 20408.01m | 80stops with 0s 0kg
 route_1:2155.409s | 17148.18m | 60stops with 0s 0kg
 route_2:2974.917s | 14349.19m | 59stops with 0s 0kg
 route_3:4342.102s | 33175.39m | 134stops with 0s 0kg
 total:12794.37s | 85080.77m | 333stops
