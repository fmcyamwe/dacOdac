# Dac O Dac

Take home Assignement

### Project Overview

.NET-based REST API that performs CRUD operations on Patient and Doctor entities.

### Technology Stack

DB: Neo4J Graph database
API: .NET API 
UI: Quasar framework based on Vue.JS

### System Architecture


### Graph Database Design
1. Constraints on Patient
   1. Patient have unique first and last names with Patient node storing a pre-normalized Upper case names used for searching and updating a patient
   2. Patient node have generated 'id' that is the Node key for lookups

2. Constraints on Doctor  
   1. Choice to have Doctor Node Key as a combination of a generated 'id', upper case last Name (stored along the original last Name) and the Doctor's 'speciality'. This to allow in multiple doctors same named doctors with different specialities.  

3. Relationships  
   1. Patient can 'Request' action from a Doctor who can agree/dismiss it(automatically creating a Doctor<>Patient) relationship
   2. When a Patient request Treatment, a Node 'Treatment' is created if it did not exist. Any existing Treatment, for versionning and surfacing a Patient's medical history, has it's relationhip set with a '.To' timestamp property.

### API Design

Effort was made to gate sensitive endpoints behing Authentication but so far it is incomplete(just gated to Listing Patients)
The UI makes requests to the 'auth/login' to renew JWT token used for request but this is done without the user's knowledge(!!bad!!)


### Web UI Design

The UI was separated in two views--the Doctor's and The patient. 
An admin Page is available to add new Patient and Doctor.

Lots of hard-coding due to lack of time :(

### Key Design Decisions

### Challenges and Solutions

Honestly **could have done** this waay faster in another language (2 or 3 days in Go) BUT the _learning_ curve of C# after __7 years__ not coding in it was much sharper than expected!

I encountered much stumbling blocks that provided some awesome learnings but without knowing how this is was evaluated, just tried to cram all the requierements focusing more on the Graph queries to have data consistency along with the UI interactions.

Admittedly the code needs lots of cleanup and with an extra day could implement it all but alas...

Attempts to have it deployed via docker images were partially successful! 

### Testing and Validation

### Future Considerations


### Appendices
