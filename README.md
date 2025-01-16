# Dac O Dac

Take home Assignment

### Project Overview

* .NET-based REST API that performs CRUD operations on Patient and Doctor entities, highlighting the relationships between these two entities.

* Data storage using Neo4j, with a schema of your own design.

* A simple web UI built using:  
      HTML5, JavaScript, and CSS; or  
      Frameworks like Bootstrap, Material UI, or a popular front-end framework such as React, Angular, or Vue.

* Secure storage and retrieval of all sensitive/personal identifying information.


### Technology Stack

+DB: Neo4J Graph database  
+API: .NET API (controller based)  
+UI: Quasar framework based on Vue.JS and placed in another [github repository](https://github.com/fmcyamwe/dacWeb)  

Deployed using Docker containers--with some issues:(see below) that I hope to fix.


### System Architecture

This was a great exercise in speed coding, setting a tight scope (to avoid some yak-shaving) and retraining muscle memory in .NET! 

My apologies if you (the reader or evaluator) encounter some Quebecois swear words when perusing my code; I usually comment out code along with the small nuggets of knowledge learned before removing them during code cleanup and copying those nuggets into personal text files (already into ~1400 lines for my personal text file for this assignment ha!)

TL;DR I started with the skeleton, installing the C# Dev Kit on my mac (probably the reason the project folder/file structure might seem different?) and getting reacquainted with an opinionated .NET. 

The web UI drives most of the interactions between the Patient and Doctor, though one can use Swagger's API page along with Neo4J Desktop to view the database state.


### Graph Database Design

String IDs were used instead of Neo4J's brittle IDs, for better robustness. These are generated via GUID on entity creation and checked for uniqueness, saving just the last 12 characters.

1. Constraints on Patient Entity
   1. Patients have unique first and last names with the Patient node also storing both pre-normalized upper cased names for searching and updating.
   2. Patient nodes have a generated 'Id' that is the Node key for lookups.--it cannot be empty and is unique.

2. Constraints on Doctor Entity  
   1. Choice was made to have the Doctor's Node key as a combination of the generated 'Id', upper cased last Name (stored along the original last Name) and the Doctor's 'speciality'. This would allow multiple doctors with the same last name but different specialities. Also the specialities are a hard-coded list that gets fetched on one of the endpoints for use in the UI.

3. Relationships  
   1. A Patient can 'Request' action from a Doctor who can agree/dismiss it (automatically creating a Doctor<>Patient) relationship. The patient can have multiple attending doctors (as in real life?)
   2. When a Patient requests Treatment, a Node 'Treatment' is created if it did not exist. Any existing Treatment, for versioning and surfacing of the Patient's medical history, has its relationship set with a '.To' end timestamp property.


### API Design

Effort was made to gate sensitive endpoints behind Authentication but so far it is incomplete (just gated to Listing Patients as test first)

The UI makes requests to the 'auth/login' endpoint for JWT tokens that is valid for an hour. But this is done without the user's knowledge(!!bad!!) it is also not a renewal as the token is not saved in the backend--just the frontend.
Plans were to implement callback for retry but as it stands, token reissual is done automatically but not the retry of any failed previous api request.

Also used controller based API divided into files by operation. Not sure if that is a .NET standard but it is easier to find an endpoint (minus those endpoints that don't belong neatly somewhere and the choice was made to keep in their respective controller file)


### Web UI Design

The UI was separated in two views--the Doctor's and the patient. At the landing page, a check is made for connecting to the API along with retrieval of random accounts that a user can use when picking the Doctor or Patient view to login.

An admin Page is available to add/delete Patients and Doctors.However it still needs some work to be presentable--hence its access is hidden as the 'Menu' icon at top left of the landing page/toolbar!

The JWT token is saved in the browser's localStorage.

Lots of hard-coding due to lack of time :(


### Key Design Decisions

The decision was made early on to have two different projects, one for the API (along any present & potential logic),
the second for the Neo4j Graph database access. Putting both in one solution might have been a drawback for deployment but I am not so sure now.

As for sensitive information, there was not much time to include much personal/sensitive information, other than names perhaps? Also chose to just use the year of birth instead of date of birth as well as forgoing Addresses,etc

So even though an API Service layer was added to potentially address this concern (with plans to also enrich model objects with metadata at this level or place restrictions for security/authentication concerns), this layer proved to be overkill for now. 

The retrieval of sensitive information was addressed at the database level with only a few gated endpoints capable of returning the full information of a Patient (i.e their medical history) with the other endpoints returning just the basic information.


### Challenges and Solutions

Honestly **could have completed** the API quickly in another language (2 or 3 days in Go) BUT the _learning_ curve of C# after __7 years__ not coding in it was much sharper than expected!

I encountered many stumbling blocks that provided some awesome learnings but without knowing how this was evaluated, I just tried to cram all the requirements in, focusing more on the Graph queries for data consistency along with the UI interactions to surface Patient-Doctor relationships as they change through time.

Attempts to have it deployed via docker images were partially successful as somewhere along the way it became too hard to debug network issues between containers and I had to move forward.

A list of roadblocks encountered:

- Dependency Injection...sheesh! (I last encountered dependency injection 7 years ago --using C# at my previous job-- kid you not!) but it seems to have become a bigger part of the language than I recall. It did prompt me to look up how it is done in other languages. For Golang, Structs and Interfaces are used clearly that it feels (imo) more natural; though I believe the manual way of creating dependencies in Go leads to less magic behind the scenes, until a project becomes complex that using libraries and toolkits is requiered, then the magic can start to be somewhat equivalent (it took me at least a day to wrap my mind around DI)

- The deserialization of database AND JSON data objects! 
 For JSON, my first attempt was using annotations, which did help, however using the NewtonsoftJson fixed that troublesome issue!
 For Neo4j node objects, it was harder to fix. My first attempt to fix this was using the 'Neo4j.Driver.Extensions' package to annotate Model classes. Already using the 'Neo4J.Driver' for database access, both packages had dependencies on another package with conflicting versions (.NETStandard 2.1 vs .NETStandard 2.0) so I had to downgrade the 'Neo4J.Driver' package version while using an alias--see Dac.Neo project file.
 This mismatch might be the cause of docker containers failing to connect? --still trying to determine this but with the recent changes to the docker compose file plus adding guardrail in the [Neo4jExtension file](Dac.Neo/Neo4jExtensions.cs), the container stays up long enough to investigate  and I surmise it's probably a port or DNS issue.
   > Update: It was a DNS issue. The API and Neo4J containers now connect successfully! 
   >> Still having issues pointing the localhost IP (localhost:5113) in the UI container to the API container. Unfortunately, when the UI is accessed from a browser, it seems a reverse proxy is needed to forward requests properly via dns! So far no luck and a nice challenge playing with nginx.

 I realize now at the end that I could have aliased Node properties instead of fetching the whole Node or even adding extension methods but alas...
 This is where my lack of familiarity was a bit crippling and as a result there are many redundant data access methods that could be merged into single queries. I also manually mapped models to/from DB models until I saw that there are mapping libraries out there!

 Please do let me know if there is a better way for this issue--I would really appreciate some feedback on this.

- Also had a couple of annoyances fighting the auto-completion in VS code editor! Using tabs was a hazard :D


### Testing and Validation

Some manual testing and validation done but definitely still a work in progress.
Used TypedResults<IResult> for endpoints return for the ease of future unit testing. Also tried to follow conventions but uncertain if I properly followed them. 


### Future Considerations

This is still a work in progress and after limiting the scope, there is still lots to do. I am certain that I shall keep working on this as leaving unfinished work is anathema with me. Plus after the early stumblings, it is much easier now with lots to learn and improve on :)

I just hope to get those pesky docker containers working in time that running them wont be an issue. I have rambled too much and it's better interacting with an app than wading through code. My apologies that I didnt even do a schema graph design but hopefully it will be there by the time you read this. 

I really would love some feedback(in any shape/form) and I thank you for looking at this.

