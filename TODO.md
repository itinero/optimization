Main TODO's:

**conceptual todo's**:

The main thing left to think about is how to elegantly handle turning costs:

- Can we combine or reuse existing methods of solving?
- Can we perhaps use a regular solver and then 'add' in turning costs after?
- Can we optmize a fixed sequence efficiëntly (related to above).

**make sure to check to TODO's in the code**

- Convert *all* TSP solvers to solvers that can handle sub-problems of matrices.
- Take into account visit costs in all solvers.
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