Main TODO's:

**make sure to check to TODO's in the code**

- Convert *all* TSP solvers to solvers that can handle sub-problems of matrices.
- Take into account visit costs in all solvers.
- Unittest solver registry and solverhooks.
- Unittest mappers.
- Unittest model validation and implement if needed.
- Make sure the public API makes sense, internalize everything except what we can use in solvers/strategies.
   - We can verify this by implement or starting a BWaste-specific implementation.
- Unittest Shared.CheapestInsertion or verify that is tested.
- Unittest Shared.HillClimbing3Opt or verify that it is tested.
