Itinero.Optimization
--------------------

A module for solving optimization routing problems with Itinero as a routing solution.

![Build status](http://build.itinero.tech:8080/app/rest/builds/buildType:(id:Itinero_LogisticsDevelop)/statusIcon)
[![Visit our website](https://img.shields.io/badge/website-itinero.tech-020031.svg) ](http://www.itinero.tech/)
[![GPL licensed](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/itinero/optimization/blob/develop/LICENSE.md)

- Itinero.Optimization: [![NuGet](https://img.shields.io/nuget/v/Itinero.Optimization.svg?style=flat)](https://www.nuget.org/packages/Itinero.Optimization/)   

### Supported problems

Current we have the following problems supported:

- TSP: The default travelling salesman problem.
- Directed TSP: The TSP with costs for turns.
- TSP-TW: The TSP with time window constraints.
- Directed TSP-TW: The TSP-TW with costs for turns.
- STSP: The selective travelling salesman problem, generates routes with as much locations as possible with a maxium travel time.
- DirectedSTSP: Identical to the STSP but with u-turn prevention.
- CVRP (experimental): The capacitated vehicle routing problem.

![Solutions](solutions.gif)

### Usage

Install the following package via Nuget:

    PM> Install-Package Itinero.Optimization -IncludePrerelease

Then immediately some extension methods are available on top the default Itinero _Router_ class. 

#### TSP, Directed TSP:  
`router.Optimize (string profileName, Coordinate[] locations)`

This calculates a TSP by default starting and ending at the location at index `0`. Specifiy `first` and `last` to change this behaviour, when leaving `last=null` the solver decides the best last location. When setting `turnPenalty` u-turns are being avoided when they introduce a cost bigger than the given penalty. 